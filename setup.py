from setuptools import setup, Extension
import pybind11
from pybind11.setup_helpers import Pybind11Extension


cpp_args=["-std=c++17"]

functions_module = Extension(
    name='MAPFSim',
    sources=['./src/wrapper.cpp',
            './src/utils.cpp',
            './src/Simulator.cpp'],
    include_dirs=[pybind11.get_include()],
    language='c++',
    extra_compile_args=cpp_args,
)



setup(ext_modules=[functions_module])