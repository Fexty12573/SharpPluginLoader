import sys
import re
import os
import shutil
from dataclasses import dataclass, field
from pathlib import Path

TYPE_MAP = {
    'u8': 'byte',
    'u16': 'ushort',
    'u32': 'uint',
    'u64': 'ulong',
    's8': 'sbyte',
    's16': 'short',
    's32': 'int',
    's64': 'long',
    'f32': 'float',
    'f64': 'double',
    'bool': 'bool',
    'void': 'void',
    'char': 'char',
    'string': 'nint',
    'cstring': 'string',
    'classref': 'nint',
    # 'class': 'nint', # Represents the vtable pointer
    'custom': 'nint',
    'color': 'MtColor',
    'point': 'MtPoint',
    'size': 'MtSize',
    'rect': 'MtRect',
    'matrix44': 'MtMatrix4X4',
    'matrix33': 'MtMatrix3X3',
    'vector2': 'MtVector2',
    'vector3': 'MtVector3',
    'vector4': 'MtVector4',
    'quaternion': 'MtQuaternion',
    'time': 'MtTime',
    'float2': 'MtFloat2',
    'float3': 'MtFloat3',
    'float4': 'MtFloat4',
    'float3x3': 'MtFloat3X3',
    'float4x3': 'MtFloat4X3',
    'float4x4': 'MtFloat4X4',
    'easecurve': 'MtEaseCurve',
    'line': 'MtLine',
    'linesegment': 'MtLineSegment',
    'ray': 'MtRay',
    'plane': 'MtPlane',
    'sphere': 'MtSphere',
    'capsule': 'MtCapsule',
    'aabb': 'MtAABB',
    'obb': 'MtOBB',
    'cyclinder': 'MtCyclinder',
    'triangle': 'MtTriangle',
    'range': 'MtRange',
    'rangef': 'MtRangeF',
    'rangeu16': 'MtRangeU16',
    'hermitecurve': 'MtHermiteCurve',
    'float3x4': 'MtFloat3X4',
    'plane_xz': 'MtPlaneXZ',
    'pointf': 'MtPointF',
    'sizef': 'MtSizeF',
    'rectf': 'MtRectF',
}

UNMANAGED_TYPES = [
    'bool',
    'u8',
    'u16',
    'u32',
    'u64',
    's8',
    's16',
    's32',
    's64',
    'f32',
    'f64',
    'string',
    'cstring',
    'classref',
    'custom'
]

@dataclass
class Field:
    name: str = ''
    type: str = ''
    offset: int = 0
    containing_class: 'Class' = None
    crc: int = 0
    flags: int = 0
    is_array: bool = False
    array_size: int = 0 # 0 = dynamic
    get: int = None
    set: int = None
    get_count: int = None
    reallocate: int = None
    comment: str = ''
    
@dataclass
class Class:
    name: str = ''
    parent: 'Class' = None
    namespaces: list[str] = field(default_factory=list)
    classes: list['Class'] = field(default_factory=list)
    fields: list[Field] = field(default_factory=list)
    vtable: int = 0
    size: int = 0
    crc: int = 0
    containing_class: 'Class' = None

CLASS_COMMENT = re.compile(r'// ([^ ]+) vftable:0x([0-9a-fA-F]+), Size:0x([0-9a-fA-F]+), CRC32:0x([0-9a-fA-F]+)') # // name vftable:0x12345678, Size:0x1234, CRC32:0x12345678
CLASS_DECL = re.compile(r'class ([^ ]+) \/\*(: ([^\*]*))?\*\/ {') # class [namespaces::]name /*: parent[, parent2, ...]*/ {
FIELD_VAR_DECL = re.compile(r'(    |\t)([^ ]+) \'([^\']+)\'(\[([^\]]+)\])? *; \/\/ Offset:0x([0-9a-fA-F]+), (Var|Array), CRC32:0x([0-9a-fA-F]+), Flags:0x([0-9a-fA-F]+)') #     type 'name'; // Offset:0x1234, Var, CRC32:0x12345678, Flags:0x100
FIELD_PROP_DECL = re.compile(r'(    |\t)([^ ]+) \'([^\']+)\'(\[([^\]]+)\])? *; \/\/ Offset:0x7FFFFFFFFFFFFFFF, (DynamicArray|PSEUDO-PROP), Getter:0x([0-9a-fA-F]+), Setter:0x([0-9a-fA-F]+),( GetCount:0x([0-9a-fA-F]+), Reallocate:0x([0-9a-fA-F]+))? CRC32:0x([0-9a-fA-F]+), Flags:0x([0-9a-fA-F]+)')

class_data_map: dict[str, list[str]] = {}
classes: dict[str, Class] = {}

indent = 0
current_file = None

def write_to_output(s: str, end = "\n"):
    global current_file, indent
    if s == '': # Newline
        current_file.write(end)
    else:
        current_file.write('    ' * indent + s + end)

print = write_to_output

def is_unmanaged_type(native_type: str) -> bool:
    return native_type in UNMANAGED_TYPES

def is_namespace(name: str) -> bool:
    return name[0] == 'n'

def get_class_name(raw_name: str):
    if raw_name == 'cpComponent':
        return 'Component'
    if raw_name.startswith('cp'):
        return raw_name[2:] + 'Component'
    if raw_name.startswith('s') and not "Manager" in raw_name:
        if raw_name in ["sApp", "sAppExt", "sMain", "sMhMain", "sWwiseDriver"]:
            return raw_name[1:]
        return raw_name[1:] + 'Manager'
    if "bitset" in raw_name:
        return raw_name.replace("<", "").replace(">", "").replace("bitset", "Bitset")
    if raw_name[0].islower():
        return raw_name[1:]
    return raw_name
    
def in_same_namespace(cls: Class, other: Class):
    if len(cls.namespaces) != len(other.namespaces):
        return False
    try:
        for i in range(len(cls.namespaces)):
            if cls.namespaces[i] != other.namespaces[i]:
                return False
    except IndexError:
        os.write(2, f'Failed to compare {get_fully_qualified_name(cls)} and {get_fully_qualified_name(other)}\n'.encode())
        return False
    return True

def is_same_containing_class(cls: Class, other: Class):
    if cls.containing_class is None or other.containing_class is None:
        return False
    return cls.containing_class.name == other.containing_class.name

def get_differing_namespaces(cls: Class, other: Class):
    if in_same_namespace(cls, other):
        return []
    for i in range(min(len(cls.namespaces), len(other.namespaces))):
        if cls.namespaces[i] != other.namespaces[i]:
            return cls.namespaces[i:]
    if len(cls.namespaces) > len(other.namespaces):
        return cls.namespaces[len(other.namespaces):]
    return cls.namespaces

def get_class_qualified_name(cls: Class):
    if cls.containing_class is None:
        return cls.name
    return f'{get_class_qualified_name(cls.containing_class)}.{cls.name}'

def get_fully_qualified_name(cls: Class):
    if len(cls.namespaces) == 0:
        return get_class_qualified_name(cls)
    return f'{".".join(cls.namespaces)}.{get_class_qualified_name(cls)}'

def get_qualified_name_relative_to(cls: Class, relative_to: Class):
    if is_same_containing_class(cls, relative_to):
        return cls.name
    if in_same_namespace(cls, relative_to):
        return get_class_qualified_name(cls)
    differing_namespaces = get_differing_namespaces(cls, relative_to)
    if len(differing_namespaces) == 0:
        return get_class_qualified_name(cls)
    return f'{".".join(differing_namespaces)}.{get_class_qualified_name(cls)}'

def get_preprocessed_class_name(name: str):
    if "<" in name:
        template = name[name.index("<") + 1:name.index(">")]
        return f"{name[:name.index('<')]}_{template.replace('::', '_')}"
    return name

def get_new_fields(cls: Class):
    if cls.parent is None:
        return cls.fields
    new_fields: list[Field] = []	
    parent_fields = [f.name for f in cls.parent.fields]
    for field in cls.fields:
        if field.name not in parent_fields:
            new_fields.append(field)
    return new_fields

def preprocess_lines(lines: list[str]):
    for i in range(len(lines)):
        line = lines[i]
        if line.startswith('//'):
            comment = re.match(CLASS_COMMENT, line)
            if not comment:
                continue

            class_lines = []
            for j in range(i + 1, len(lines)):
                class_line = lines[j]
                if class_line.startswith('//'):
                    break
                class_lines.append(class_line)
            
            class_data_map[comment.group(1)] = [comment] + class_lines


def process_field(line: str, owner: Class, comment: str = '') -> Field:
    if '0x7FFFFFFFFFFFFFFF' in line:
        prop = re.match(FIELD_PROP_DECL, line)
        if not prop:
            raise Exception(f'Failed to parse property: {line}')

        name = prop.group(3).replace(' ', '')
        if "[" in name:
            name = name[:name.index("[")]
        if "::" in name:
            name = name[name.index("::") + 2:]
        if prop.group(6) == 'DynamicArray':
            return Field(
                name=name,
                type=prop.group(2),
                offset=-1,
                containing_class=owner,
                crc=int(prop.group(12), 16),
                flags=int(prop.group(13), 16),
                is_array=True,
                array_size=0,
                get=int(prop.group(7), 16),
                set=int(prop.group(8), 16),
                get_count=int(prop.group(10), 16),
                reallocate=int(prop.group(11), 16),
                comment=comment
            )
        else:
            return Field(
                name=name,
                type=prop.group(2),
                offset=-1,
                containing_class=owner,
                crc=int(prop.group(12), 16),
                flags=int(prop.group(13), 16),
                is_array=False,
                array_size=0,
                get=int(prop.group(7), 16),
                set=int(prop.group(8), 16),
                comment=comment
            )
    else:
        var = re.match(FIELD_VAR_DECL, line)
        if not var:
            raise Exception(f'Failed to parse variable: {line}')

        name = var.group(3).replace(' ', '')
        if "[" in name:
            name = name[:name.index("[")]
        if "::" in name:
            name = name[name.index("::") + 2:]
        return Field(
            name=name,
            type=var.group(2),
            offset=int(var.group(6), 16),
            containing_class=owner,
            crc=int(var.group(8), 16),
            flags=int(var.group(9), 16),
            is_array=var.group(7) == 'Array',
            array_size=int(var.group(5), 16) if var.group(5) else 0,
            comment=comment
        )


def process_class(cls: Class, lines: list[str | re.Match[str]]):
    global classes
    last_comment = ''
    for line in lines:
        if isinstance(line, re.Match):
            cls.vtable = int(line.group(2), 16)
            cls.size = int(line.group(3), 16)
            cls.crc = int(line.group(4), 16)
            continue
            
        if line.startswith('class'):
            decl = re.match(CLASS_DECL, line)
            if not decl:
                continue

            to_simple_name = lambda s: get_class_name(get_preprocessed_class_name(s).split("::")[-1])

            name = get_preprocessed_class_name(decl.group(1))
            names = name.split('::')
            parent = decl.group(3).split(', ')[0] if decl.group(3) else None
            cls.name = get_class_name(names[-1])

            if parent:
                simple_name = to_simple_name(parent)
                if simple_name in classes:
                    cls.parent = classes[simple_name]
                elif parent in class_data_map: # Parent not found, add it to the list of classes to be processed
                    cls.parent = Class(name=parent)
                    classes[simple_name] = cls.parent
                    process_class(cls.parent, class_data_map[parent])
                else: # Parent was not dumped by DTI dumper
                    cls.parent = None

            cls.namespaces = [ns[1:] for ns in names[:-1] if is_namespace(ns)]

            containing_class = None
            if len(names) > 1 and not is_namespace(names[-2]):
                containing_class = get_class_name(names[-2])
                if containing_class in classes:
                    classes[containing_class].classes.append(cls)
                    cls.containing_class = classes[containing_class]
                elif names[-2] in class_data_map:
                    classes[containing_class] = Class(name=containing_class)
                    classes[containing_class].classes.append(cls)
                    process_class(classes[containing_class], class_data_map[names[-2]])
                    cls.containing_class = classes[containing_class]
                else:
                    cls.containing_class = Class(name=containing_class, parent=None, namespaces=cls.namespaces, classes=[cls],\
                                                 fields=[], vtable=0, size=0, crc=0, containing_class=None)
                    classes[containing_class] = cls.containing_class
            continue
        elif line.startswith('}'):
            return
        elif line.strip().startswith('//'):
            last_comment = line[2:].strip()
            continue

        if "0x7FFFFFFFFFFFFFFF" in line and ("Var" in line or 'Array' in line): # Usually event properties, ignore these
            continue
        cls.fields.append(process_field(line, cls, last_comment))


def process(file: str):
    with open(file, 'r') as f:
        preprocess_lines(f.readlines()[2:])
    
    for name, lines in class_data_map.items():
        if name in classes:
            continue
        if "ace" in name:
            continue

        cls = Class(name=name)
        process_class(cls, lines)
        classes[cls.name] = cls

def generate_class(cls: Class):
    global indent
    name = cls.name[1:] if cls.name[0].islower() else cls.name
    parent = get_qualified_name_relative_to(cls.parent, cls) if cls.parent else "NativeWrapper"

    print(f'public unsafe class {name} : {parent}')
    print('{')
    indent += 1

    print(f'public new static readonly uint Size = {cls.size};')
    print(f'public new static readonly uint Crc = 0x{cls.crc:X};')
    print(f'public new static readonly long VTable = 0x{cls.vtable:X};')
    print('')

    for sub in cls.classes:
        generate_class(sub)
        print('')
    
    # ------------------------------------------------------------
    # Internal struct that maps to the native structure in memory
    # ------------------------------------------------------------
    # print(f'[StructLayout(LayoutKind.Explicit)]')
    # print(f'private unsafe struct Internal{name}')
    # print('{')
    # indent += 1
    # for field in cls.fields:
    #     if field.offset == -1:
    #         continue
    #     if field.type not in TYPE_MAP:
    #         print(f'[FieldOffset(0x{field.offset:X})] public byte {field.name}; // TODO: Fix type')
    #     else:
    #         if field.is_array:
    #             if field.array_size == 0:
    #                 continue
    #             print(f'[FieldOffset(0x{field.offset:X})] public fixed {TYPE_MAP[field.type]} {field.name}[{field.array_size}];')
    #         else:
    #             print(f'[FieldOffset(0x{field.offset:X})] public {TYPE_MAP[field.type]} {field.name};')
    #     print('')
    # indent -= 1
    # print('}')

    print('')
    print(f'public {name}(nint instance) : base(instance) {{  }}')
    print(f'public {name}() : base() {{ }}')
    print('')
    print('')

    for field in get_new_fields(cls):
        if cls.parent is not None and field in cls.parent.fields:
            continue
        if field.offset == -1:
            continue # TODO: Implement properties
        if field.comment != '':
            print(f'// {field.comment}')

        field_name = field.name.split('.')[0]
        if not field_name.islower():
            field_name = field.name[1:] if field.name[0] == 'm' else field.name
            while field_name[0].islower():
                field_name = field_name[1:]

        if field.type not in TYPE_MAP:
            print(f'public byte {field_name} {{ get => Get<byte>({hex(field.offset)}) set => Set<byte>({hex(field.offset)}, value); }}')
        else:
            mapped_type = TYPE_MAP[field.type]
            if field.is_array:
                if field.array_size == 0:
                    continue
                if is_unmanaged_type(field.type):
                    print(f'public ref {mapped_type} {field_name}(int index) => ref GetRef<{mapped_type}>({hex(field.offset)} + (index * sizeof({mapped_type})));')
                else:
                    print(f'public ref {mapped_type} {field_name}(int index) => ref GetMtTypeRef<{mapped_type}>({hex(field.offset)} + (index * sizeof({mapped_type})));')
            else:
                if is_unmanaged_type(field.type):
                    print(f'public {mapped_type} {field_name} {{ get => Get<{mapped_type}>({hex(field.offset)}); set => Set<{mapped_type}>({hex(field.offset)}, value); }}')
                else:
                    print(f'public {mapped_type} {field_name} {{ get => GetMtType<{mapped_type}>({hex(field.offset)}); set => SetMtType<{mapped_type}>({hex(field.offset)}, value); }}')
        print('')

    indent -= 1
    print('}')
    

def generate_class_file(cls: Class):
    global current_file, indent
    indent = 0

    if "ace" in cls.namespaces:
        return
    
    if cls.containing_class is not None:
        return

    file = './Generated/Mt/'
    if len(cls.namespaces) > 0:
        file += '/'.join(cls.namespaces) + '/'
    file += get_class_name(cls.name) + '.cs'
    os.makedirs(os.path.dirname(file), exist_ok=True)

    current_file = open(file, 'w')
    print('using System;')
    print('using System.Runtime.InteropServices;') # For StructLayout/FieldOffset
    print('')
    if len(cls.namespaces) > 0:
        print(f'namespace SharpPluginLoader.Core.Mt.{".".join(cls.namespaces)}')
    else:
        print('namespace SharpPluginLoader.Core.Mt')
    print('{')
    indent += 1

    generate_class(cls)
    indent -= 1
    print('}')
    print('')

    current_file.close()


def generate_classes():
    if Path('./Generated').exists():
        shutil.rmtree('./Generated')

    for cls in classes.values():
        generate_class_file(cls)

if __name__ == '__main__':
    process(sys.argv[1])
    generate_classes()
