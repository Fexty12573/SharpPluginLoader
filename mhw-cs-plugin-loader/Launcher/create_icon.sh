#! /usr/bin/env sh
# -define icon:auto-resize=256,128,96,64,48,32,16
magick -density 512 -define icon:auto-resize=32 -background none ../../docs/images/SPL.svg Launcher.ico
