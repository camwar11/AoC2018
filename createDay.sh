#!/bin/sh 
mkdir ./src/$1/$2
cd ./src/$1/$2
dotnet new console
cd ../..
dotnet sln AoC.sln add ./$1/$2/$2.csproj
dotnet add ./$1/$2/$2.csproj reference ./common/common.csproj