#include "utils.hpp"
#include <fstream>
#include <regex>



ACTION string_to_action(std::string as){
    if(as=="F") return ACTION::FORWARD;
    if(as=="CCW") return ACTION::COUNTERCLOCK;
    if(as=="CW") return ACTION::CLOCKWISE;
    if(as=="W") return ACTION::WAIT;
    if(as=="U") return ACTION::UP;
    if(as=="L")return ACTION::LEFT;
    if(as=="R") return ACTION::RIGHT;
    if(as=="D") return ACTION::DOWN; 
}

/// @brief 
/// @param map_file 
/// @return 
std::tuple<int,int,std::vector<std::pair<int,int>>>read_map(std::string map_file)
{
    int xmax,ymax;
    std::vector<std::pair<int,int>> obstacles;
    std::ifstream file(map_file);
    std::string line;
    std::smatch results;
    std::regex r_height = std::regex(R"(height\s(\d+))");
    std::regex r_width = std::regex(R"(width\s(\d+))");
    std::regex r_map = std::regex(R"(map)");
    // fundamental graph params
    while (std::getline(file, line))
    {
        // for CRLF coding
        if (*(line.end() - 1) == 0x0d)
            line.pop_back();

        if (std::regex_match(line, results, r_height))
        {
            ymax = std::stoi(results[1].str());
        }
        if (std::regex_match(line, results, r_width))
        {
            xmax = std::stoi(results[1].str());
        }
        if (std::regex_match(line, results, r_map))
            break;
    }
    if (!(xmax > 0 && ymax > 0))
        throw std::exception("map invalid");

    // create nodes
    int y = 0;
    // V = Nodes(width * height, nullptr);
    while (getline(file, line))
    {
        // for CRLF coding
        if (*(line.end() - 1) == 0x0d)
            line.pop_back();

        if ((int)line.size() != xmax)
            throw std::exception("map invalid");
        for (int x = 0; x < xmax; ++x)
        {
            char s = line[x];
            if (s == 'T' || s == '@')
            {
                obstacles.push_back({x, y});
                continue; // object
            }

            // int id = xmax * y + x;
            // Node *v = new Node(id, x, y);
            // std::cout<<*v<<std::endl;
            // V[id] = v;
        }
        ++y;
    }
    if (y != ymax)
        throw std::exception("map invalid");
    file.close();
    return {xmax,ymax,obstacles};
}

/// @brief
/// @param starts
/// @param goals
std::tuple<std::vector<std::pair<int,int>>,std::vector<std::pair<int,int>>>read_instance(std::string instance_file)
{
    std::string line;
    std::ifstream file(instance_file);
    std::smatch results;
    std::regex r_sg=std::regex(R"((\d+) (\d+) (\d+) (\d+))");
    std::regex r_comment=std::regex(R"(#.+)");
    std::vector<std::pair<int,int>> starts,goals;
    while (getline(file, line))
    {
        // CRLF
        if (*(line.end() - 1) == 0x0d)
            line.pop_back();
        // comment
        if (std::regex_match(line, results, r_comment))
        {
            continue;
        }

        // read map
        // read initial/goal nodes
        if (std::regex_match(line, results, r_sg))
        {
            int x_s = std::stoi(results[1].str());
            int y_s = std::stoi(results[2].str());
            int x_g = std::stoi(results[3].str());
            int y_g = std::stoi(results[4].str());
            starts.push_back({x_s,y_s});
            goals.push_back({x_g,y_g});
            continue;
        }
    }
    return {starts,goals};
}

/// @brief
/// @param path
/// @return
std::vector<ACTION> pos_to_actions(const std::vector<std::pair<int, int>> &path)
{
    using point2d=std::pair<int,int>;
    if(path.empty()) return {};
    std::vector<ACTION> result;
    int x_pre=path[0].first;
    int y_pre=path[0].second;
    for(int k=1;k<path.size();k++){
        int x_curr=path[k].first;
        int y_curr=path[k].second;
        // assert(abs(x_curr-x_pre)+abs(y_curr-y_pre)<=1);
        if(x_curr-x_pre==1)result.push_back(ACTION::RIGHT);
        if(x_curr-x_pre==-1) result.push_back(ACTION::LEFT);
        if(y_curr-y_pre==1) result.push_back(ACTION::UP);
        if(y_curr-y_pre==-1) result.push_back(ACTION::DOWN);
        if(y_curr==y_pre &&x_curr==x_pre) result.push_back(ACTION::WAIT);
        x_pre=x_curr;
        y_pre=y_curr;
    }
    return result;

}

/// @brief 
/// @param s 
/// @param delim 
/// @param out 
void tokenize(std::string const &s, std::string const & delim,
            std::vector<std::string> &out){
 
    auto start = 0U;
    auto end = s.find(delim);
    std::string token;
    while (end!= std::string::npos) {
        token = s.substr(start, end - start);
        start = end + delim.length();
        // std::cout << token << std::endl;
        out.push_back(token);
        end = s.find(delim, start);
    }
    token = s.substr(start, end - start);
    out.push_back(token);

}


/// @brief 
/// @param paths_file 
/// @param paths 
std::vector<std::vector<std::pair<int,int>>> read_paths(std::string paths_file){
    std::ifstream sol_file(paths_file);
    std::string line;
    std::regex r_comp_time=std::regex(R"(comp_time=(\d+))");
    std::smatch results;
    std::cout<<"read from "<<paths_file<<std::endl;
    std::vector<std::vector<std::pair<int,int>>>paths;
    int k=0;
    using strings=std::vector<std::string>;
    while(getline(sol_file,line)){
        //CRLF
        if(*(line.end()-1)==0x0d) line.pop_back();
        strings substrs;
        tokenize(line,":",substrs);
        if(substrs.size()<2) continue;
        std::vector<std::pair<int,int>> path_i;
        paths.push_back(path_i);
        // for(auto s:substrs) std::cout<<s<<std::endl;
        auto path_string=substrs[1];
        strings vertices;
        tokenize(path_string,"),",vertices);
        for(int i=0;i<vertices.size()-1;i++){
            std::string v=vertices[i];
            // std::cout<<v<<" ";
            v.erase(0,1);
            // std::cout<<v<<" \n";
            strings xy;
            tokenize(v,",",xy);
            // std::cout<<xy[0]<<" ,"<<xy[1]<<std::endl;
            paths[k].push_back({std::stoi(xy[0]),std::stoi(xy[1])});
        }
        k++;
    }
    std::cout<<"num agents="<<paths.size()<<std::endl;
    return paths;
}


/// @brief 
/// @param actions_file 
/// @return 
std::vector<std::vector<ACTION>> read_actions(std::string actions_file){
    std::ifstream sol_file(actions_file);
    std::string line;
    std::regex r_comp_time=std::regex(R"(comp_time=(\d+))");
    std::smatch results;
    std::cout<<"read from "<<actions_file<<std::endl;
    std::vector<std::vector<ACTION>>actions;
    int k=0;
    using strings=std::vector<std::string>;
    while(getline(sol_file,line)){
        //CRLF
        if(*(line.end()-1)==0x0d) line.pop_back();
        strings substrs;
        tokenize(line,":",substrs);
        if(substrs.size()<2) continue;
        std::vector<ACTION> action_i;
        actions.push_back(action_i);
        // for(auto s:substrs) std::cout<<s<<std::endl;
        auto path_string=substrs[1];
        strings vertices;
        tokenize(path_string,",",vertices);
        for(int i=0;i<vertices.size()-1;i++){
            std::string v=vertices[i];
            // std::cout<<v<<" ";
            // std::cout<<v<<" \n";
           
            // std::cout<<xy[0]<<" ,"<<xy[1]<<std::endl;
            actions[k].push_back(string_to_action(v));
        }
        k++;
    }
    std::cout<<"num agents="<<actions.size()<<std::endl;
    return actions;
}