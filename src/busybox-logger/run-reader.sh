#!/bin/sh

echo "Log: $LOG_FILE";
touch $LOG_FILE

tail -f $LOG_FILE