##################################
# Similar to a makefile but with less idiosyncrasies and some extra functionalities. 
# For more info refer to: https://github.com/casey/just
##################################

# Shows this help message
help:
    just --list

# Package the project for the provided configuration
pack configuration="Release":
    dotnet pack event-manager.sln --configuration {{configuration}}