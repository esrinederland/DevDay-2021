import arcpy
arcpy.env.overwriteOutput = True

# Note: Script assumes data from Pro SDK community samples are installed under C:\Data, as follows:
inFC = r"E:\GISTech\2021\ProProjects\PythonUsage\PythonUsage.gdb\FCL_Lijn"
outFC = r"E:\GISTech\2021\ProProjects\PythonUsage\PythonUsage.gdb\ViaScript"

# Buffer the input features creating three buffer distance feature classes
arcpy.Buffer_analysis(inFC, outFC, "500 meter")

# The following message will be included in the message box from the calling button's OnClick routine
print("Python script uitgevoerd.")