import http from 'k6/http';
import { sleep } from 'k6';
// export const options = {
//     rate: 410,
//     duration: '1m',
//     timeUnit: '1s',
//     preAllocatedVUs: 50,
//     maxVUs: 40
// };

export const options = {
    scenarios: {
        contacts: {
            executor: 'constant-arrival-rate',
            rate: 1000, // 10 RPS, т.е. 10 запросов в секунду
            timeUnit: '1s', // рассчитано в секундах
            duration: '100m', // продолжительность выполнения теста
            preAllocatedVUs: 100, // количество виртуальных пользователей, которые k6 будет поддерживать
            maxVUs: 400, // максимальное количество VUs, которые k6 может создать во время теста
        }
    }
};
export default function() {
    // http.get('http://localhost:5256/WeatherForecast');
    // http.get('http://localhost:80/WeatherForecast');
    // const authToken = 'eyJ0eXAiOiJjbGllbnQtand0IiwiYWxnIjoiUlMyNTYifQ.eyJ0ZW5hbnQiOiJUZXN0LXN0YWdpbmciLCJleHAiOjE3MDA3NjIzMTcsInBlcm1pc3Npb25zIjpbIiJdLCJmdWxsQWNjZXNzIjoiVHJ1ZSIsImlzcyI6Im1pbmRib3gifQ.25MxtF43RqLYLNTtm4_wukdWCnFpGN_rjPg8yU31Tr1I53eNilgARdsWyQNAUzVAS-haa9N1f4Tqptc_wAPG2h9DRAxZFo1OF9vUiXWOMqZY5JDeA41PDgN7L5pA1aln7-P-ICSSV6iDyvYSJ98rW6fa1g1siUnoeIqOMEGfeEc9Oh-O-eG8eKZ9sm_6QCW6nqNdK63duF_QzXxjwBlNEYaukQ7q3y3-LFQVgj0sVFLV_6p6pXUB1ByODTPrRSx8yV5sKfeTMmMKSvOMv3_eHnruYY5ob97VZfuBMVYI8s_ADpQNMPY7Uu5aZzM-JSbnLzHD1ApWIACVbuY7aDh58c96ElPaH3dHEggQxliFRdKnhJ22LvtY5FrrYAZDV1hSiEdNgoA8bxkTfpnaGYE_XxzyWJQaWpYOt7lFTgaSgPnipzt1guLau4FnjTdL9bqDvkVqCMiB8Lkqlphk2Vq51myIJ_KTWVL_7mCP_oz2gGIEeFfA7YtFFIePw8GPb_jUh0BaceQV7gtk1Xbxl5mGcd2tKCWQdGc0LPEALcMo2rAjFe63m0f-lsi2-OY5pKJ6vRxh1VTClACT6EdveK15MHt8U4sSs0LggeBNGxnxiacfDogm1PPcSWQeFGQBgKgJu_1r7MRA00WZcYKu1LB86C7ezPvqhKC6ueFk3BrijkU'; 
    // const url = 'http://reco-staging.mindbox.ru/Test-staging/mechanics/Integratsionnietesti/recommendations/for-customer?brandSystemName=Test-staging_';  // Replace with the actual URL of the API endpoint

    // const headers = {
    //     'Authorization': `Bearer ${authToken}`,
    //     'Content-Type': 'application/json',
    // };

    const payload = {
        // Your request payload goes here, if needed
    };

    // const response = http.get(url, JSON.stringify(payload), { headers });
    
    // http.get('http://localhost:1234/SimulatedStorage/GetData');
    http.get('http://localhost:80/WeatherForecast');
    // http.get('http://localhost:5189/SimulatedStorage/GetData');
    sleep(1);
}
