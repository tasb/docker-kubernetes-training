FROM busybox

WORKDIR /script

COPY run-cycle.sh .

RUN chmod +x run-cycle.sh

CMD ["/script/run-cycle.sh", "/script/log.txt" ]
