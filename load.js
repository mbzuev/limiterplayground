import http from 'k6/http';
import { sleep } from 'k6';
export const options = {
    rate: 100,
    duration: '30m',
    timeUnit: '1s',
    preAllocatedVUs: 10,
    vus: 200
};
export default function () {
    // http.get('http://localhost:5256/WeatherForecast');
    http.get('http://localhost:80/WeatherForecast');
    // http.get('http://localhost:5189/SimulatedStorage/GetData');
    sleep(1);
}
