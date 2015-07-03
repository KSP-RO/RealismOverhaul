import os 
import sys
"""
This script is used to convert engine configs from the old EngineIgnitor configuration to the new one integrated in RealFuel. This is done in the process of adapting RealismOverhaul to KSP version 1.04.

This script has only been tested on Linux (specifically, Arch). No guarantee it'll work anywhere else (the paths might break).
"""

failed_configs = []

def convert_file(filepath):
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
        if changed:
            with open(filepath, 'w') as file:
                for l in lines:
                    file.write(l + "\n")
    except:
        #We even store exceptions. We're awesome!
        print("EXCEPTION!")
        failed_configs.append(filepath)

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
    
def extract_bool(l):
    """
    Extracts a boolean from text.
    """
    if l.split("=")[1].strip().lower() == "true":
        return True
    else:
        return False

def convert_all_files(root_dir):
    """
    Walks recursively through root_dir and all subdirs, converting all files ending in ".cfg".
    Note that there may be problems with SmokeScreen configs and stuff, which isn't really my problem. I've defined a command line interface, haven't I?
    """
    for root, dirs, files in os.walk(root_dir):
        for file in files:
            if file[-4:] == ".cfg":
                convert_file(os.path.join(root, file))
                
    print("Exceptions:")
    for e in failed_configs:
        print(e)


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
        convert_file(arg)
    else:
        convert_all_files(arg)