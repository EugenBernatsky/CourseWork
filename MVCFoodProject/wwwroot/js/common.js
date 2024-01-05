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

        this._axios = axios.create({
            baseURL: this._apiBase
        });

        
        if (options.isAuth) {
            this._axios.interceptors.request.use(function (config) {
                const token = `Bearer ${getCookie("identity")}`;
                config.headers.Authorization = token;
               
                return config;
            });
        }
    }

    post(path, body) {
        return this._axios.post(path, body);
    }

    get(path) {
        return this._axios.get(path)
    }
}


class ProductsView {
    constructor(options = {}) {
        this._init(options);
    }

    _init(options) {
        this._container = options.container;
        this._variant = options.variant;
    }

    _generateAdminBadges(isActive) {
        if (isActive) {
            return '<span class="badge rounded-pill bg-success">Активний</span>'
        } else {
            return '<span class="badge rounded-pill bg-danger">Вимкнений</span>'
        }
    }

    _generateAdminProductButtonGroup(isActive, id) {
        if (isActive) {
            return `<button type="button" class="btn btn-danger" onclick="onToggleProduct(event)" data-action="disactivate" data-product-id="${id}">Вимкнути</button>`
        } else {
            return `<button type="button" class="btn btn-success" onclick="onToggleProduct(event)"  data-action="activate" data-product-id="${id}">Увімкнути</button>`
        }
    }

    _getTemplate({ id, productName, quantity, image, price, description, isActive }) {
        if (this._variant === 'cartProduct') {
            return `
               <div class='cart-product-item' data-id='${id}'>
                        <div class='cart-product-img'>
                        <img src='${image}'/>
                        </div>
                        <span class='cart-product-name'>${productName}</span>
                    
                    <span class='cart-product-delete-btn' data-product-id='${id}' onclick="deleteProductFromCart(event)">Delete</span>
                        <span class='cart-product-price'> 
                        ${price * quantity} ₴
                        </span>

                        <div class='cart-product-quantity'> 
                        <button data-product-id='${id}' onclick="incrementCartCount(event)"> +</button>  
                           <span> ${quantity} x</span>
                        <button data-product-id='${id}' onclick="decrementCartCount(event)" > -</button>
                    </div>
                </div>
            `
        }

        if (this._variant === 'mainView') {
            return `
                <div class="product-card" data-id="${id}">
                        <div class="product-img">
                            <img src="${image}" />
                        </div>
                        <div class="product-name">
                            ${productName}
                        </div>
                        <div class="product-description">
                            ${description}
                        </div>
                        <div class="product-footer">
                            <div class="product-price">
                                ${price} ₴
                            </div>
                            <div class="product-order-btn">
                                <button onclick="addProductToCart(event)" data-product-id="${id}">Обрати</button>
                            </div>
                        </div>
                </div>
            `
        }

        if (this._variant === 'adminProductCard') {
            return `
                        <img src="${image}" class="card-img-top" alt="${productName}">
                        <div class="card-body">
                            <h5 class="card-title">${productName}</h5>
                            <p class="card-text">${description}</p>
                        </div>
                        <ul class="list-group list-group-flush">
                            <li class="list-group-item">Ціна: ${price} </li>
                            <li class="list-group-item">
                                ${this._generateAdminBadges(isActive)}
                            </li>
                        </ul>
                        <div class="card-body">
                            ${this._generateAdminProductButtonGroup(isActive, id)}
                            <button type="button" onclick="editProductHandler(event)" class="btn btn-primary" data-product-id="${id}">Редагувати</button>
                        </div> 
            `;
        }

        if (this._variant === 'adminMainView') {
            return `
                    <div class="col col-4 d-flex justify-content-center">
                       <div class="card " style="width: 25rem;" data-product-id="${id}">
                        <img src="${image}" class="card-img-top" alt="${productName}">
                        <div class="card-body">
                            <h5 class="card-title">${productName}</h5>
                            <p class="card-text">${description}</p>
                        </div>
                        <ul class="list-group list-group-flush">
                            <li class="list-group-item">Ціна: ${price} </li>
                            <li class="list-group-item">
                                ${this._generateAdminBadges(isActive)}
                            </li>
                        </ul>
                        <div class="card-body">
                            ${this._generateAdminProductButtonGroup(isActive, id)}
                            <button type="button" onclick="editProductHandler(event)" class="btn btn-primary" data-product-id="${id}">Редагувати</button>
                        </div> 
                        </div>
                        </div>
            `;
        }
    }

    render(products) {
        this._products = products;

        this._container.innerHTML = '';
        this._container.innerHTML = this._products.map(p => this._getTemplate.call(this, p)).join(' ');
    }
}