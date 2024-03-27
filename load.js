import http from 'k6/http';
import { sleep } from 'k6';

export const options = {
    scenarios: {
        contacts: {
            executor: 'constant-arrival-rate',
            rate: 75,
            timeUnit: '1s',
            duration: '100m',
            preAllocatedVUs: 4000,
            maxVUs: 4500,
        }
    }
};
export default function() {
    http.get('http://localhost:80/TestRequest/Limited');
    sleep(1);
}
