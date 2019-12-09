axios.defaults.timeout = 50000;
axios.defaults.baseURL = '';
//http request 拦截器
axios.interceptors.request.use(
    config => {
        config.headers = {
            //'Content-Type': 'application/json'
            'Content-Type': 'application/x-www-form-urlencoded; charset=UTF-8',
            'x-requested-with': 'XMLHttpRequest'
            //'Access-Control-Allow-Origin': '*'
        };
        return config;
    },
    error => {
        return Promise.reject(error);
    }
);
//http response 拦截器
axios.interceptors.response.use(
    response => {
        return response;
    },
    error => {
        //判断请求超时
        if (error.code === 'ECONNABORTED' && error.message.indexOf('timeout') !== -1) {
            $.Alert({ resultSign: 1, message: "请求超时！" });
        }
        //if (error.response.status===401) {
        //    $.Alert({ resultSign: 1, message: "请求问题了！" });
        //}
        return Promise.reject(error);
    }
);
/**
* 封装get方法
* @param url
* @param data
* @returns {Promise}
*/
function $fetch(url, params = {}) {
    return new Promise((resolve, reject) => {
        axios.get(url, {
            params: params
        })
            .then(response => {
                resolve(response.data);
            })
            .catch(err => {
                reject(err);
            });
    });
};


/**
 * 封装post请求
 * @param url
 * @param data
 * @returns {Promise}
 */
function $post(url, data = {}) {
    return new Promise((resolve, reject) => {
        axios.post(url, data)
            .then(response => {
                resolve(response.data);
            }, err => {
                reject(err);
            });
    });
}

/**
* 封装patch请求
* @param url
* @param data
* @returns {Promise}
*/
function $patch(url, data = {}) {
    return new Promise((resolve, reject) => {
        axios.patch(url, data)
            .then(response => {
                resolve(response.data);
            }, err => {
                reject(err)
            })
    })
}
/**
* 封装put请求
* @param url
* @param data
* @returns {Promise}
*/
function $put(url, data = {}) {
    return new Promise((resolve, reject) => {
        axios.put(url, data)
            .then(response => {
                resolve(response.data);
            }, err => {
                reject(err)
            })
    })
}
/**
* 封装delete请求
* @param url
* @param data
* @returns {Promise}
*/
function $delete(url, data = {}) {
    return new Promise((resolve, reject) => {
        axios.delete(url, {
            params: data
        })
            .then(response => {
                resolve(response.data);
            }, err => {
                reject(err)
            })
    })
}