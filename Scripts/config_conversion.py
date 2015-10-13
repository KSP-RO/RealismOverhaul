import os 
import sys
import traceback
"""
This script is used to convert engine configs from the old EngineIgnitor configuration to the new one integrated in RealFuel. This is done in the process of adapting RealismOverhaul to KSP version 1.04.

This script has only been tested on Linux (specifically, Arch). No guarantee it'll work anywhere else (the paths might break).
"""

failed_configs = []

def convert_file(filepath, change_function):
    """
    This converts a single file defined by filepath.
    """
    try:
        changed = False
        print("Converting %s" %filepath)
        # First, we read in the file.
        with open(filepath,'r') as file:
            lines = read_lines(file)
        # With the lines stored locally, we can now search for EngineIgnitor configs.
        lines, changed = change_function(lines)
        if changed:
            with open(filepath, 'w') as file:
                for l in lines:
                    file.write(l + "\n")
    except Exception as e:
        #We even store exceptions. We're awesome!
        print("EXCEPTION!")
        
        print(traceback.format_exc())
        failed_configs.append(filepath)
                
def get_end_brackets(lines, start_line):
    """
    Gets the end bracket for the one starting in start_line. Works with nested ones, but only with brackets alone in the line.
    """
    opened_brackets = 1
    end_line = start_line+1
    while opened_brackets > 0:
        end_line += 1
        if lines[end_line].strip() == "{":
            opened_brackets += 1
        elif lines[end_line].strip() == "}":
            opened_brackets -= 1
    return end_line+1
            
def read_lines(file):
    # returns the lines of the file (without \n at the end.)
    lines = [line.rstrip('\n') for line in file]
    return lines
    
def extract_bool(l):
    """
    Extracts a boolean from text.
    """
    if l.split("=")[1].strip().lower() == "true":
        return True
    else:
        return False

def convert_all_files(root_dir, change_function):
    """
    Walks recursively through root_dir and all subdirs, converting all files ending in ".cfg".
    Note that there may be problems with SmokeScreen configs and stuff, which isn't really my problem. I've defined a command line interface, haven't I?
    """
    for root, dirs, files in os.walk(root_dir):
        for file in files:
            if file[-4:] == ".cfg":
                convert_file(os.path.join(root, file), change_function)
                
    print("Exceptions:")
    for e in failed_configs:
        print(e)