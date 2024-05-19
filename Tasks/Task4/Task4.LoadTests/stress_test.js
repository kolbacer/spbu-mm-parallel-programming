import http from 'k6/http';
import { URL } from 'https://jslib.k6.io/url/1.0.0/index.js';

const timeout = 5000;

export const options = {
    discardResponseBodies: true,
    scenarios: {
        stress_mixed: {
            exec: "stressTestMixed",
            executor: 'ramping-vus',
            startVUs: 100,
            stages: [
                { duration: '10s', target: 1000 },
                { duration: '20s', target: 5000 },
                { duration: '30s', target: 10000 },
                { duration: '1m', target: 20000 },
                { duration: '10s', target: 0 },
            ],
            gracefulRampDown: '0s',
        },
    },
    thresholds: {
        http_req_duration: [{ threshold: `p(95) < ${timeout}`, abortOnFail: true }],
    },
};

const baseURL = "http://localhost/api/ExamSystem/"

const endpoints = {
    count: "Count",
    contains: "Contains",
    add: "Add",
    remove: "Remove"
}

const methodRatios = [
    {method: endpoints.add, ratio: 0.09},
    {method: endpoints.contains, ratio: 0.9},
    {method: endpoints.remove, ratio: 0.01}
];

function getRandomInt(max) {
    return Math.floor(Math.random() * max);
}

function getRandomMethod() {
    let rnd = Math.random();
    let cumulativeRatio = 0;

    for (let i = 0; i < methodRatios.length; i++) {
        cumulativeRatio += methodRatios[i].ratio;
        if (rnd < cumulativeRatio)
            return methodRatios[i].method;
    }

    throw new Error("Incomplete distribution");
}

function getRandomParamsRequest(endpoint) {
    let studentId = getRandomInt(10000);
    let courseId = getRandomInt(10000);
    let url = new URL(baseURL + endpoint);
    url.searchParams.append("studentId", studentId);
    url.searchParams.append("courseId", courseId);
    return url.toString();
}

export function stressTestMixed() {
    let method = getRandomMethod();
    let request = getRandomParamsRequest(method);
    switch (method) {
        case endpoints.add:
            http.post(request);
            break;
        case endpoints.contains:
            http.get(request);
            break;
        case endpoints.remove:
            http.del(request);
            break;
    }
}