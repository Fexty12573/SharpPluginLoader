using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpPluginLoader.Core.Fsm.Weapon;

/// <summary>
/// Represents an FSM condition evaluator function. See <see cref="FsmExtender"/> for more information.
/// </summary>
/// <param name="transitionObject">The transition object that contains some state information</param>
/// <returns>-1 if the condition is not met, and 0 or greater if the condition is met</returns>
/// <remarks>
/// The object passed to this delegate is always a cHumanTransitionBase object, unless you
/// registered your condition with a custom DTI. In that case, the object will be a pointer to
/// an instance of the class represented by that DTI.
/// </remarks>
public delegate int FsmConditionDelegate(nint transitionObject);
