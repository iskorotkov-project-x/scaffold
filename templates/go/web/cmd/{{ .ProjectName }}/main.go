package main

import (
	"fmt"
	"log"
	"net/http"
	"time"
)

const addr = ":8881"

func main() {
	http.HandleFunc("/", handle)

	log.Printf("Server starting listening on %s\n", addr)

	if err := http.ListenAndServe(addr, nil); err != nil {
		log.Fatal(err)
	}
}

func handle(w http.ResponseWriter, r *http.Request) {
	fmt.Fprintf(w, "Hello World!\nIt's %v today.", time.Now().Format("Monday"))
}
