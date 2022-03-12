#!/bin/sh

if [ -z "$URL_TEST" ] 
then
    echo "Set env var URL_TEST to execute the loop"
    exit 0
fi

echo "Run load on $URL_TEST";

while sleep 0.01
do 
  wget -q -O- $URL_TEST
  echo ""
done
