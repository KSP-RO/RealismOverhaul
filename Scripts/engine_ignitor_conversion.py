import os 
import sys
from config_conversion import *
"""
This script is used to convert engine configs from the old EngineIgnitor configuration to the new one integrated in RealFuel. This is done in the process of adapting RealismOverhaul to KSP version 1.04.

This script has only been tested on Linux (specifically, Arch). No guarantee it'll work anywhere else (the paths might break).
"""

def change_ignitor_config(lines):
    for i, l in enumerate(lines):
            #just found a module with name.
            if "name = ModuleEngineIgnitor" == l.strip():
                start_line = i - 2 # This is the "MODULE" line.
                end_line = get_end_brackets(lines, start_line)
                print("\tModuleEngineIgnitor found in lines %i-%i." %(start_line, end_line))
                module = read_in_module(lines[start_line:end_line+1])
                del lines[start_line:end_line] #we'll delete the old lines.
                placement_line = find_module_placement(lines[: start_line]) -1 #the new placement line is directly above the closing } of the MODULE[ModuleEngines] block.
                tab_count = lines[placement_line-1].count("\t") #make sure we've got the formatting right.
                lines[placement_line:placement_line] = gen_new_module(module, tab_count)
                changed = True
                
            #found cloned part, that is a definition inside an engine config.
            elif "ModuleEngineIgnitor" == l.strip():
                start_line = i
                end_line = get_end_brackets(lines, start_line)
                print("\tcloned ModuleEngineIgnitor found in lines %i-%i." %(start_line, end_line))
                module = read_in_module(lines[start_line:end_line+1])
                del lines[start_line:end_line]
                placement_line = start_line # here, we place it on its old line.
                tab_count = lines[placement_line-1].count("\t")
                lines[placement_line:placement_line] = gen_new_module(module, tab_count)
                changed = True
            elif "!MODULE[ModuleEngineIgnitor]" in l.strip():
                del lines[i]
                changed = True
    return lines, changed

def gen_new_module(module, tab_count=0):
    """
    Generates a list of lines from the "module" definition.
    Module can define the following:
        - ullage
        - pressure_fed
        - ignitions
        - several ignitor resources.
    """
    list = []
    tabs = "\t"*tab_count
    if "ullage" in module:
        list.append(tabs + "%%ullage = %r" %module["ullage"])
    if "pressure_fed" in module:
        list.append(tabs + "%%pressureFed = %r" %module.get("pressure_fed", True))
    if "ignitions" in module:
        list.append(tabs + "%%ignitions =%s" %module.get("ignitions", 1))
    list.append(tabs + "!IGNITOR_RESOURCE,* {}")
    if "ignitor_resources" in module:
        for resource in module["ignitor_resources"]:
            for i, l in enumerate(resource):
                if i in [0, 1, len(resource)-1]:
                    list.append(tabs + l)
                else:
                    list.append(tabs + "\t" + l)
    return list
            
def find_module_placement(lines):
    """
    Finds the correct location for inserting the module in. Give it a truncated lines list (ending where the module was placed originally), and you'll get the line after which to add it. It's looking for a bracket to end one of the statements in the if clause.
    """
    for i, l in enumerate(reversed(lines)):
        if "@MODULE[ModuleEngines*]" in l or "@MODULE[ModuleEngines]" in l or "@[ModuleEngines*]" in l:
            return get_end_brackets(lines, len(lines)-i)
                
def read_in_module(lines):
    """
    Reads in an old module and returns a dict module.
    Module can define the following:
        - ullage
        - pressure_fed
        - ignitions
        - several ignitor resources.
    """
    module = {}
    module["ignitor_resources"] = []
    for i, l in enumerate(lines):
        if "ignitionsAvailable" in l:
            module["ignitions"] = l.split("=")[1]
        if "useUllageSimulation" in l:
            module["ullage"] = extract_bool(l)
            
        if "isPressureFed" in l:
            module["pressure_fed"] = extract_bool(l)
        if "IGNITOR_RESOURCE" in l:
            start_line = i
            end_line = get_end_brackets(lines, start_line)
            module["ignitor_resources"].append(extract_ignitor_resource(lines[start_line:end_line]))
    return module
    
def extract_ignitor_resource(lines):
    """
    Extracts an ignitor resource from the lines. We just copy each line (without tabs) and return the old list, since nothing in that definition has changed.
    """
    list = []
    for i, l in enumerate(lines):
        list.append(l.strip())
    return list

#We ignore all command line arguments except for the first one.
if len(sys.argv) > 1:
    arg = sys.argv[1]
else:
    arg = "."
if arg == "-h":
    print("This script is used to convert engine configs from the old EngineIgnitor configuration to the new one integrated in RealFuel. This is done in the process of adapting RealismOverhaul to KSP version 1.04.")
    print("To use, specify the filepath for a directory to convert configs in; in this and all subdirectories, all found configs will be updated. If you do not specify any, this script will continue from the current path. If the specified path ends in \".cfg\", we will only convert this single file.")
else:
    if arg[-4:] == ".cfg":
        convert_file(arg, change_ignitor_config)
    else:
        convert_all_files(arg, change_ignitor_config)