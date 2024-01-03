// Global Variables 
const API_BASE = 'https://localhost:7280'
//

function getCookie(name) {
    const value = `; ${document.cookie}`;
    const parts = value.split(`; ${name}=`);
    if (parts.length === 2) return parts.pop().split(';').shift();
}

class AuthProvider {

    setIdentity(token) {
        document.cookie = `identity=${token}; path=/;`
    }

    getIdentity() {
        return getCookie('identity');
    }
}

class UserProfile extends AuthProvider {
    constructor() {
        super();
    }

    setUserProfile(user) {
        this._desirialize(user); 
    }

    _desirialize(DTO) {
        super.setIdentity(DTO.token);
        this.role = DTO.role;
        this.userId = DTO.userId;
    }
}

const USER_INSTANCE = new UserProfile();

class ApiClient {
    constructor(options = {}) {
        this._apiBase = options.apiBase;
    }

    post(path, options = {}) {
        return this._requestHelper(path, {
            ...options, method: "POST", headers: {
                "Content-Type": "application/json"
            }
        })
            .then(response => {
                const contentType = response.headers.get("content-type");

                if (contentType && contentType.indexOf("application/json") !== -1) {
                    return response.json();
                }
            })
    }

    get(path, options = {}) {
        return this._requestHelper(path, { ...options })
            .then(response => {
                const contentType = response.headers.get("content-type");

                if (contentType && contentType.indexOf("application/json") !== -1) {
                    return response.json();
                }
            })
    }

    _requestHelper(path, options) {
        const obj = {};

        if (options.isAuth) {
            obj.headers = {
                "Authorization": `Bearer ${getCookie("identity")}`
            };
        }

        if (options.headers) {
            obj.headers = {
                ...(obj.headers || {}),
                ...options.headers
            }
        }

        if (options.method) {
            obj.method = options.method;
        }

        if (options.body) {
            obj.body = JSON.stringify(options.body);
        }
        

        return fetch(`${this._apiBase}${path}`, obj);
    }
}