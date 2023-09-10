import axios from 'axios';

const API = axios.create({
    baseURL : "http://localhost:5124/api"
})

export default API;