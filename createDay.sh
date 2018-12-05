#!/bin/sh 
mkdir ./src/$1
cd ./src/$1
dotnet new console
cd ..
dotnet sln AoC.sln add ./$1/$1.csproj
dotnet add ./$1/$1.csproj reference ./common/common.csproj