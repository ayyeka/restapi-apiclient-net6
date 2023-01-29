package main

import (
	"encoding/json"
	"io"
	"log"
	"net/http"
	"net/url"
	"strings"
)

type AccessToken struct {
	Token   string `json:"access_token"`
	Expires int    `json:"expires_in"`
}

type Stream struct {
	Id     int    `json:"id"`
	SiteId int    `json:"site_id"`
	Name   string `json:"display_name"`
}

func getAccessToken() (string, error) {
	client := http.Client{}
	data := url.Values{}
	data.Set("grant_type", "client_credentials")
	AYYEKA_URL := goDotEnvVariable("AYYEKA_URL")
	ACCOUNT_KEY := goDotEnvVariable("ACCOUNT_KEY")
	ACCOUNT_SECRET := goDotEnvVariable("ACCOUNT_SECRET")

	req, err := http.NewRequest(http.MethodPost, AYYEKA_URL+"/auth/token", strings.NewReader(data.Encode()))
	if err != nil {
		return "Request not created", err
	}

	req.Header.Add("Content-Type", "application/x-www-form-urlencoded")
	req.SetBasicAuth(ACCOUNT_KEY, ACCOUNT_SECRET)

	res, err := client.Do(req)
	if err != nil {
		return "Token not created", err
	}

	defer res.Body.Close()

	resBody, err := io.ReadAll(res.Body)
	if err != nil {
		log.Fatal(err)
	}

	var emp AccessToken
	json.Unmarshal(resBody, &emp)

	return emp.Token, nil
}

func getStreams(token string) ([]Stream, error) {
	client := http.Client{}

	AYYEKA_URL := goDotEnvVariable("AYYEKA_URL")
	req, err := http.NewRequest(http.MethodGet, AYYEKA_URL+"/v2.0/stream", nil)
	if err != nil {
		return []Stream{}, err
	}

	req.Header.Add("Authorization", token)

	res, err := client.Do(req)
	if err != nil {
		return []Stream{}, err
	}

	defer res.Body.Close()

	resBody, err := io.ReadAll(res.Body)
	if err != nil {
		log.Fatal(err)
	}

	var streams []Stream
	json.Unmarshal(resBody, &streams)

	return streams, nil
}
