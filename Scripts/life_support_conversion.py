from config_conversion import *


TAC_resource_names = ["Food", "Water", "Oxygen", "CarbonDioxide", "Waste", "WasteWater", "LithiumHydroxide"]


def change_life_support_config(lines):
    changed_modules = []
    changed = False
    for i, l in enumerate(lines):
        # look for a tank with one of TAC's resources
        if "name =" == l.strip()[:6]:
            if l.strip()[6:].strip() in TAC_resource_names:
                changed = True
                #we found it! Now, let's find the @Part line.
                part_line = find_part_line(lines[:i+1])
                if lines[part_line] not in changed_modules:
                    changed_modules.append(lines[part_line])
                    end_part_line = get_end_brackets(lines, part_line)
                    #now that we found the start and end line, let's look for the @MODULE[ModuleCommand]
                    module_command_line_start, module_command_line_end = find_module_command(lines[part_line:end_part_line])
                    
                    if module_command_line_start is not None:
                        module_command_line_start += part_line
                        module_command_line_end += part_line
                        # here we modify the line.
                        lines[module_command_line_start:module_command_line_end] = modify_command_module(lines[module_command_line_start:module_command_line_end])
                    else:
                        placement_line = end_part_line-1
                        tabs = lines[placement_line-1].count("\t")
                        lines[placement_line:placement_line] = gen_module_generator(tabs=tabs)
                    
    return lines, changed

def gen_module_generator(tabs=0):
    """
    
	MODULE
	{
		name = ModuleGenerator
		isAlwaysActive = true
		OUTPUT_RESOURCE
		{
			name = ElectricCharge
			rate = 0.00075
		}
	}
    """
    strings = []
    strings.append("MODULE")
    strings.append("{")
    strings.append("\tname = ModuleGenerator")
    strings.append("\tisAlwaysActive = true")
    strings.append("\tOUTPUT_RESOURCE")
    strings.append("\t{")
    strings.append("\t\tname = ElectricCharge")
    strings.append("\t\trate = -0.2 //200W for life support base")
    strings.append("\t}")
    strings.append("}")
    for i, s in enumerate(strings):
        strings[i] = "\t"*tabs + strings[i]
    return strings

def modify_command_module(lines):
    for i, l in enumerate(lines):
        if l.strip() == "name = ElectricCharge":
            # found it, changing rate
            tabs = l.count("\t")
            rate = modify_rate(lines[i+1].strip()[7:])
            lines[i+1] = "\t"*tabs + "rate = " + rate + "//200W for life support base"
    return lines

def modify_rate(string):
    float_number = float(string)
    float_number += 0.2
    if "." in string:
        accuracy = max(1, len(string.split(".")[1]))
    else:
        accuracy = 1
    return str(round(float_number, accuracy))
    #whole_numbers, part_numbers = string.split(".")
    #if part_numbers[0] not in ["0", "1"]:
    #    #the simple case.
    #    part_numbers[0] = int(part_numbers[0])-2
    #else:
    #    if whole_numbers
    #string = whole_numbers + "." + part_numbers
    #return string

def find_part_line(lines):
    for i, l in enumerate(reversed(lines)):
        if "@PART" == l.strip()[:5]:
            return len(lines)-i
    raise Exception("Could not find associated part.")

def find_module_command(lines):
    for i, l in enumerate(lines):
        if l.strip() == "@MODULE[ModuleCommand]":
            module_command_line_start = i
            module_command_line_end = get_end_brackets(lines, i+1)
            return module_command_line_start, module_command_line_end
    return None, None
            


#We ignore all command line arguments except for the first one.
if len(sys.argv) > 1:
    arg = sys.argv[1]
else:
    arg = "."
if arg == "-h":
    print("")
    print("To use, specify the filepath for a directory to convert configs in; in this and all subdirectories, all found configs will be updated. If you do not specify any, this script will continue from the current path. If the specified path ends in \".cfg\", we will only convert this single file.")
else:
    if arg[-4:] == ".cfg":
        convert_file(arg, change_life_support_config)
    else:
        convert_all_files(arg, change_life_support_config) 
