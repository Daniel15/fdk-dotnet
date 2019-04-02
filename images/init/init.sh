#!/bin/sh
mkdir content
sed s/{FUNCTION_NAME}/$FN_FUNCTION_NAME/ template/Dockerfile.in > content/Dockerfile
sed s/{FUNCTION_NAME}/$FN_FUNCTION_NAME/ template/Program.cs.in > content/Program.cs
cp template/Project.csproj "content/$FN_FUNCTION_NAME.csproj"
cp template/func.init.yaml content/func.init.yaml
tar -C content -cf init.tar .

cat init.tar

rm -rf content
rm init.tar