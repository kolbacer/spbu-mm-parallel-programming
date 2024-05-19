import http from 'k6/http';
import { URL } from 'https://jslib.k6.io/url/1.0.0/index.js';

const duration = 30;
const rate1 = 30;
const rate2 = 100;
const initVUs = 10;
const maxVUs = 50;

// scenarios run sequentially
export const options = {
    summaryTimeUnit: 'ms',
    scenarios: {
        load_30rps_add: {
            exec: "addTest",
            executor: 'constant-arrival-rate',
            rate: rate1,
            timeUnit: '1s',
            duration: `${duration}s`,
            gracefulStop: '0s',
            startTime: '0s',
            preAllocatedVUs: initVUs,
            maxVUs: maxVUs,
        },
        load_30rps_contains: {
            exec: "containsTest",
            executor: 'constant-arrival-rate',
            rate: rate1,
            timeUnit: '1s',
            duration: `${duration}s`,
            gracefulStop: '0s',
            startTime: `${duration}s`,
            preAllocatedVUs: initVUs,
            maxVUs: maxVUs,
        },
        load_30rps_remove: {
            exec: "removeTest",
            executor: 'constant-arrival-rate',
            rate: rate1,
            timeUnit: '1s',
            duration: `${duration}s`,
            gracefulStop: '0s',
            startTime: `${duration*2}s`,
            preAllocatedVUs: initVUs,
            maxVUs: maxVUs,
        },
        load_100rps_add: {
            exec: "addTest",
            executor: 'constant-arrival-rate',
            rate: rate2,
            timeUnit: '1s',
            duration: `${duration}s`,
            gracefulStop: '0s',
            startTime: `${duration*3}s`,
            preAllocatedVUs: initVUs,
            maxVUs: maxVUs,
        },
        load_100rps_contains: {
            exec: "containsTest",
            executor: 'constant-arrival-rate',
            rate: rate2,
            timeUnit: '1s',
            duration: `${duration}s`,
            gracefulStop: '0s',
            startTime: `${duration*4}s`,
            preAllocatedVUs: initVUs,
            maxVUs: maxVUs,
        },
        load_100rps_remove: {
            exec: "removeTest",
            executor: 'constant-arrival-rate',
            rate: rate2,
            timeUnit: '1s',
            duration: `${duration}s`,
            gracefulStop: '0s',
            startTime: `${duration*5}s`,
            preAllocatedVUs: initVUs,
            maxVUs: maxVUs,
        },
    }
};

const baseURL = "http://localhost/api/ExamSystem/"

const endpoints = {
    count: "Count",
    contains: "Contains",
    add: "Add",
    remove: "Remove"
}

function getRandomInt(max) {
    return Math.floor(Math.random() * max);
}

function getRandomParamsRequest(endpoint) {
    let studentId = getRandomInt(10000);
    let courseId = getRandomInt(10000);
    let url = new URL(baseURL + endpoint);
    url.searchParams.append("studentId", studentId);
    url.searchParams.append("courseId", courseId);
    return url.toString();
}

export function addTest() {
    http.post(getRandomParamsRequest(endpoints.add));
}

export function containsTest() {
    http.get(getRandomParamsRequest(endpoints.contains));
}

export function removeTest() {
    http.del(getRandomParamsRequest(endpoints.remove));
}
