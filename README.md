AutoFindMaterials - an asset postprocessor for 3D model assets
it searches the project for materials first getting rid of the suffix
default separator is dot - because Blender's default auto-naming is *.001, *.002 etc

model's import settings should be set to "Import via MaterialDescription"
(consider making it a Unity Preset and set it as default in Project Settings)
"Import" action on a model asset will run this postprocessor
