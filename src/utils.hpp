#pragma once
#include"actions.hpp"
#include <iostream>
#include<vector>
#include "json.hpp"

std::tuple<int,int,std::vector<std::pair<int,int>>>read_map(std::string map_file);
std::tuple<std::vector<std::pair<int,int>>,std::vector<std::pair<int,int>>>read_instance(std::string instance_file);
std::vector<std::vector<std::pair<int,int>>> read_paths(std::string paths_file);
std::vector<std::vector<ACTION>> read_actions(std::string actions_file);
std::vector<ACTION> pos_to_actions(const std::vector<std::pair<int,int>>&);
