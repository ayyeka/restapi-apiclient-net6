package main

import (
	"fmt"
	"os"
)

func main() {

	token, err := getAccessToken()

	if err != nil {
		fmt.Println("Not able to create token")
		os.Exit(1)
	}

	fmt.Println(token)

	streams, err := getStreams(token)

	if err != nil {
		fmt.Println("Not able to fetch streams")
		os.Exit(1)
	}

	fmt.Println(streams)
}
