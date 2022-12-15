cmake_minimum_required(VERSION 3.16)
project(mapf_python)
set(pybind11_DIR C:/Users/GREATEN/AppData/Local/Programs/Python/Python310/share/cmake/pybind11)
find_package(pybind11 REQUIRED)
find_package(Python COMPONENTS Interpreter Development REQUIRED)

set(CMAKE_CXX_STANDARD 17 CACHE STRING "C++ version selection")  # or 11, 14, 17, 20

# set(CMAKE_BUILD_TYPE Debug)
IF(NOT CMAKE_BUILD_TYPE)
    SET(CMAKE_BUILD_TYPE "RELEASE")
ENDIF()
set(CMAKE_CXX_FLAGS_DEBUG "$ENV{CXXFLAGS} -O0  -g2 -ggdb")
set(CMAKE_CXX_FLAGS_RELEASE "$ENV{CXXFLAGS} -O3")




pybind11_add_module(MAPFSim  "./src/Simulator.cpp" "./src/wrapper.cpp" "./src/utils.cpp")
# target_link_libraries(Grids PRIVATE GRAPHLIB)





