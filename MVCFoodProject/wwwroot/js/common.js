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

    delete(path) {
        return this._axios.delete(path);
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
            return '<span class="badge rounded-pill bg-success">Active</span>'
        } else {
            return '<span class="badge rounded-pill bg-danger">Inactive</span>'
        }
    }

    _generateAdminProductButtonGroup(isActive, id) {
        if (isActive) {
            return `<button type="button" class="btn btn-warning" onclick="onToggleProduct(event)" data-action="disactivate" data-product-id="${id}">Turn off</button>`
        } else {
            return `<button type="button" class="btn btn-success" onclick="onToggleProduct(event)"  data-action="activate" data-product-id="${id}">Turn on</button>`
        }
    }

    _getTemplate({ id, productName, quantity, image, price, description, isActive }) {
        if (this._variant === "cartProducts") {
            return `
                <div class="col col-12" data-id="${id}">
                  <div class="row">
                                <div class="col col-2 d-flex align-items-center">
                                    <div>
                                        <img src="${image}" class="img-fluid" alt="">
                                    </div>
                                </div>
                                <div class="col col-4 d-flex align-items-center">
                                    ${productName}
                                </div>
                                <div class="col col-3 d-flex align-items-center">
                                    <span class='cart-product-quantity'>
                                        ${price * quantity} ₴
                                    </span>
                                </div>
                                <div class="col col-2 d-flex justify-content-center cart-product-quantity">
                                    <div class="btn-group-vertical " role="group" aria-label="Vertical button group">
                                        <button type="button" class="btn btn-primary" data-product-id='${id}' onclick="incrementCartCount(event)">+</button>
                                        <button type="button" class="btn btn-light" disabled><span>${quantity} x</span></button>
                                        <button type="button" class="btn btn-primary" data-product-id='${id}' onclick="decrementCartCount(event)" >-</button>
                                    </div>
                                </div>
                                <div class="col col-1 d-flex justify-content-center align-items-center">
                                    <button type="button" class="btn btn-danger" data-product-id='${id}' onclick="deleteProductFromCart(event)">X</button>
                                </div>
                            </div>           
                </div>
            `
        }

        if (this._variant === 'mainView') {
            return `
                <div class="col col-4 d-flex justify-content-center">
                    <div class="card " style="width: 25rem;" data-product-id="${id}">
                        <img style="height: 300px; max-width: 100%; object-fit: contain;" src="${image}" class="card-img-top" alt="${productName}">
                        <div class="card-body">
                            <h5 class="card-title">${productName}</h5>
                            <p class="card-text">${description}</p>
                        </div>
                        <ul class="list-group list-group-flush">
                            <li class="list-group-item">Price: ${price} </li>

                        </ul>
                        <div class="card-body">
                            <button type="button" onclick="addProductToCart(event)" data-product-id="${id}" class="btn btn-primary">Choose</button>
                        </div>
                    </div>
                </div>
            `
        }

        if (this._variant === 'adminProductCard') {
            return `
                        <img src="${image}" style="height: 300px; max-width: 100%; object-fit: contain;" class="card-img-top" alt="${productName}">
                        <div class="card-body">
                            <h5 class="card-title">${productName}</h5>
                            <p class="card-text">${description}</p>
                        </div>
                        <ul class="list-group list-group-flush">
                            <li class="list-group-item">Price: ${price} </li>
                            <li class="list-group-item">
                                ${this._generateAdminBadges(isActive)}
                            </li>
                        </ul>
                        <div class="card-body">
                            ${this._generateAdminProductButtonGroup(isActive, id)}
                            <button type="button" onclick="editProductHandler(event)" class="btn btn-primary" data-product-id="${id}">Edit</button>
                            <button type="button" class="btn btn-danger" onclick="onDeleteProduct(event)" data-product-id="${id}">Delete</button>
                        </div> 
            `;
        }

        if (this._variant === 'adminMainView') {
            return `
                    <div class="col col-4 d-flex justify-content-center">
                       <div class="card " style="width: 25rem;" data-product-id="${id}">
                        <img src="${image}" style="height: 300px; max-width: 100%; object-fit: contain;" class="card-img-top" alt="${productName}">
                        <div class="card-body">
                            <h5 class="card-title">${productName}</h5>
                            <p class="card-text">${description}</p>
                        </div>
                        <ul class="list-group list-group-flush">
                            <li class="list-group-item">Price: ${price} </li>
                            <li class="list-group-item">
                                ${this._generateAdminBadges(isActive)}
                            </li>
                        </ul>
                        <div class="card-body">
                            ${this._generateAdminProductButtonGroup(isActive, id)}
                            <button type="button" onclick="editProductHandler(event)" class="btn btn-primary" data-product-id="${id}">Edit</button>
                            <button type="button" class="btn btn-danger" onclick="onDeleteProduct(event)" data-product-id="${id}">Delete</button>
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

class EditProfile {
    constructor (options) {
        this._editor = options.editor;
        this._openEditor = options.openEditor;
        this._modalSelector = options.modalSelector;

        this._onSuccsess = options.onSuccsess;
        this._onFail = options.onFail

        this._init();

        this._apiClient = new ApiClient({
            apiBase: API_BASE,
            isAuth: true
        });
    }

    onSubmit() {
        const formData = new FormData(this._editor);
        const payload = new FormData();

        const tmpUserName = formData.get('name');
        const tmpUserNumber = formData.get('number');
        const tmpUserAddress = formData.get('adress');
        const tmpImage = this._tmpImg;


        if (this._currentName !== tmpUserName) {
            payload.append('name', tmpUserName);
        }

        if (this._currentNumber !== tmpUserNumber) {
            payload.append('number', tmpUserNumber || '');
        }

        if (this._currentAddress !== tmpUserAddress) {
            payload.append('address', tmpUserAddress || '');
        }

        if (tmpImage && this._currentImg !== tmpImage) {
            payload.append('image', tmpImage);
        }

        if (!payload.entries().next().value) {
            this._modal.hide();
            return;
        }

        const buttons = this._editor.querySelectorAll('.handler');

        buttons?.forEach(b => b.setAttribute("disabled", true));

        this._apiClient.post('/user/edit', payload)
            .then(res => {
                if (this._onSuccsess) {
                    this._onSuccsess(res)
                }
            })
            .catch(e => {
                if (this._onFail) {
                    this._onFail(e)
                }
            }).finally(() => {
                buttons?.forEach(b => b.removeAttribute("disabled"));
                this._modal.hide();
                this._tmpImg = null;
            });
    }

    _init() {
        this._modal = new bootstrap.Modal(this._modalSelector, {
            keyboard: false
        });

        this._imageEditor = this._editor.querySelector('#uplod-image');

        this._imageEditor.addEventListener('change', () => {
            const image = this._editor.querySelector('.image-preview img');

            if (!event.target.files[0] || !image) {
                return;
            }

            this._tmpImg = event.target.files[0];
            image.onload = () => {
                URL.revokeObjectURL(image.src);
            }

            image.src = URL.createObjectURL(this._tmpImg);
        });



        this._openEditor.addEventListener('click', () => {
            this._modal.show();
            const existedData = new FormData(this._editor);

            this._currentName = existedData.get('name');
            this._currentNumber = existedData.get('number');
            this._currentAddress = existedData.get('adress');
            this._currentImg = this._editor.querySelector('.image-preview img').src;
        });

        this._editor.addEventListener('submit', (e) => {
            e.preventDefault();
            this.onSubmit();
        });
    }
}

