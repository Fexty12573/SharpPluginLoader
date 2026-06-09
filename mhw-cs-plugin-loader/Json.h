#pragma once

#define JSON_NOEXCEPTION 1
#include <nlohmann/json.hpp>

using json = nlohmann::ordered_json;
