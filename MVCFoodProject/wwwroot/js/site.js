
class ProductViewModel {
    constructor(apiClient = new ApiClient({ apiBase: API_BASE })) {
        this._pizzaProducts = [];
        this._sushiProducts = [];
        this._drinksProducts = [];
        this._apiClient = apiClient;

        this._cartProducts = [];
    }

    initViewModels() {
        this._pizzaViewInstance = new ProductsView({
            container: document.querySelector('.container-pizza'),
            variant: 'mainView'
        });

        this._sushiViewInstance = new ProductsView({
            container: document.querySelector('.container-sushi'),
            variant: 'mainView'
        });

        this._sushiSetsViewInstance = new ProductsView({
            container: document.querySelector('.container-sushisets'),
            variant: 'mainView'
        });

        this._salatsViewInstance = new ProductsView({
            container: document.querySelector('.container-salat'),
            variant: 'mainView'
        });

        this._drinksViewInstance = new ProductsView({
            container: document.querySelector('.container-drinks'),
            variant: 'mainView'
        });

        this._cartViewInstance = new ProductsView({
            container: document.querySelector('.cart-products-list'),
            variant: 'cartProducts'
        });
    }

    async fetchProducts() {
        const products = await this._apiClient.get('/getListProducts');
        this._parseProductsByCategory(products);
    }

    async fetchProductsById(ids) {
        const { data } = await this._apiClient.get(`/getListProductsById?ids=${ids.join(',')}`);

        this._cartProducts = data.map(this._productMapper)
    }

    _productMapper(product) {
        return {
            id: product.internalId,
            productName: product.productsDetails.productName,
            description: product.productsDetails.description,
            price: product.productsDetails.price,
            image: product.productsDetails.imgURL
        };
    }

    _parseProductsByCategory(products) {
        products.forEach(p => {
            const mappedProduct = this._productMapper(p);

            if (p.categoryType === 'PIZZA') {
                this._pizzaProducts.push(mappedProduct);
            }

            if (p.categoryType === 'SUSHI') {
                this._sushiProducts.push(mappedProduct);
            }

            if (p.categoryType === 'DRINKS') {
                this._drinksProducts.push(mappedProduct);
            }
        })
    }

    getView(key) {
        return this[`_${key}ViewInstance`];
    }

    getPizzaProducts() {
        return this._pizzaProducts;
    }

    getSushiProducts() {
        return this._sushiProducts;
    }

    getDrinksProducts() {
        return this._drinksProducts;
    }

    getCartProducts() {
        return this._cartProducts;
    }

    getAllProducts() {
        return [...this._pizzaProducts, ...this._sushiProducts, this._drinksProducts];
    }
}

const productViewModelInstance = new ProductViewModel();

const CartModal = new bootstrap.Modal('.modal-cart', {
    keyboard: false
});

const AuthModal = new bootstrap.Modal('.auth-modal', {
    keyboard: false
});

const OrderAccepted = new bootstrap.Modal('.order-accepted', {
    keyboard: false,
});


const loginForm = document.querySelector('#login-form');
const registerForm = document.querySelector('#register-form');
const authWrapper = document.querySelector('.auth-wrapper');

const origin = window.location.origin;
const DESTINATION_MAP_BY_ROLE = {
    admin: 'adminpage',
    customer: 'customerpage',
    courier: 'courierpage',
    unknown: 'foodpage'
}

loginForm.addEventListener('submit', (e) => {
    const api = new ApiClient({ apiBase: API_BASE });
    e.preventDefault();

    const data = new FormData(loginForm);

    const payload = {
        useremail: data.get('email'),
        password: data.get('password'),

    }

    api.post('/login', payload)
        .then(({ data: user }) => {
            USER_INSTANCE.setUserProfile(user);
            authWrapper.innerHTML = `<button type="button" class="btn btn-light" onclick="onRedirect('${user.role}')">Мій профіль</button>`;
            AuthModal.hide();
        })
        .catch(e => {
            console.error(e)
        });
});

registerForm.addEventListener('submit', (e) => {
    const api = new ApiClient({ apiBase: API_BASE });
    e.preventDefault();

    const data = new FormData(registerForm);

    const payload = {
        username: data.get('name'),
        password: data.get('password'),
        email: data.get('email'),
        phone: data.get('phone'),
        address: data.get('address')
    }


    api.post('/register', payload)
        .then(({ data: user }) => {
            USER_INSTANCE.setUserProfile(user);
            authWrapper.innerHTML = `<button type="button" class="btn btn-light" onclick="onRedirect('${user.role}')">Мій профіль</button>`;
            AuthModal.hide();
        })
        .catch(e => {
            console.error(e)
        });
});

document.addEventListener('DOMContentLoaded', function () {
    productViewModelInstance.initViewModels();

    initCartCount();
    
});


function onRedirect(role) {
    window.location.replace(`${origin}/${DESTINATION_MAP_BY_ROLE[role]}`)
}


function initCartCount() {
    const cartProducts = JSON.parse(localStorage.getItem('cartProducts')) || {};

    const productCount = Object.values(cartProducts).reduce((acc, value) => {
        return acc + value.quantity;
    }, 0)

    if (productCount) {
        incrementProductCount(productCount);
    }
}


const { increment: incrementProductCount, decrement: decrementProductCount, cleaunUp: cleaunUpProductCount } = function () {
    const cartCounters = document.querySelectorAll('.cart-items-count');

    const increment = (value) => {
        cartCounters.forEach(ck => ck.innerHTML = `(${value})`);
    };

    const decrement = (value) => {
        cartCounters.forEach(ck => ck.innerHTML = `(${value})`);
    }

    const cleaunUp = () => {
        cartCounters.forEach(ck => ck.innerHTML = '');
    }

    return {
        increment,
        decrement,
        cleaunUp
    }
}();

const { increment: incrementCartCount, decrement: decrementCartCount, deleteCartProduct: deleteProductFromCart } = function () {
    const buttons = document.querySelector('.checkout-button');

    const handler = (type, event) => {
        const productId = event.target.getAttribute('data-product-id');
        const cartProducts = JSON.parse(localStorage.getItem('cartProducts')) || {};

        if (type === 'increment') {
            cartProducts[productId] = { ...cartProducts[productId], quantity: cartProducts[productId].quantity + 1 };
        }

        if (type === 'decrement') {
            if (!(cartProducts[productId].quantity - 1)) {
                delete cartProducts[productId];
            } else {
                cartProducts[productId] = { ...cartProducts[productId], quantity: cartProducts[productId].quantity - 1 };
            }
        }

        if (type === 'delete') {
            if (cartProducts[productId]) {
                delete cartProducts[productId];
            }
        }

        const fillteredProducts = productViewModelInstance.getCartProducts().reduce((acc, p) => {
            if (cartProducts[p.id]) {
                return acc.concat({
                    ...p,
                    quantity: cartProducts[p.id].quantity
                });
            }
            return acc;
        }, [])

        const viewInstance = productViewModelInstance.getView('cart');

        localStorage.setItem('cartProducts', JSON.stringify(cartProducts));

        const productCount = Object.values(cartProducts).reduce((acc, value) => {
            return acc + value.quantity;
        }, 0);

        viewInstance.render(fillteredProducts);

        if (type === 'increment') {
            incrementProductCount(productCount);
        }

        if (type === 'decrement' || type === 'delete') {
            if (!productCount) {
                cleaunUpProductCount();
                buttons?.setAttribute('disabled', true);
            } else {
                decrementProductCount(productCount)
            }
        }

        countCartTotalPrice(!Boolean(productCount));
    }

    return {
        increment: handler.bind(null, 'increment'),
        decrement: handler.bind(null, 'decrement'),
        deleteCartProduct: handler.bind(null, 'delete')
    }
}();

function addProductToCart(event) {
    const productId = event.target.getAttribute('data-product-id');
    const cartProducts = JSON.parse(localStorage.getItem('cartProducts')) || {};

    cartProducts[productId] = cartProducts[productId] ? { ...cartProducts[productId], quantity: cartProducts[productId].quantity + 1 } : { quantity: 1 };
    localStorage.setItem('cartProducts', JSON.stringify(cartProducts));

    const productCount = Object.values(cartProducts).reduce((acc, value) => {
        return acc + value.quantity;
    }, 0)

    incrementProductCount(productCount);
}

function onOpenCartModal() {
    CartModal.show();

    const buttons = document.querySelector('.checkout-button');
    buttons?.setAttribute('disabled', true);
    const productsFromStorage = JSON.parse(localStorage.getItem('cartProducts'));
    const viewInstance = productViewModelInstance.getView('cart');
 
    if (!productsFromStorage || !Object.keys(productsFromStorage).length) {
        viewInstance.render([]);
        return;
    }

    productViewModelInstance.fetchProductsById(Object.keys(productsFromStorage)).then(() => {
        const fillteredProducts = productViewModelInstance.getCartProducts().reduce((acc, p) => {
            return acc.concat({
                ...p,
                quantity: productsFromStorage[p.id].quantity
            });

        }, [])

        
        if (fillteredProducts.length) {
            viewInstance.render(fillteredProducts);
            buttons?.removeAttribute('disabled');
        }

        countCartTotalPrice(!Boolean(fillteredProducts.length));
    });
}

function countCartTotalPrice(isCleanUp = false) {
    const el = document.querySelector('.cart-total-price');

    if (isCleanUp) {
        el.innerHTML = '';
        return;
    }

    const productsFromStorage = JSON.parse(localStorage.getItem('cartProducts'));

    if (productsFromStorage && !Object.keys(productsFromStorage).length) {
        el.innerHTML = `Загальна сумма ${0} ₴`;
        return;
    }

    const cartTotalSum = productViewModelInstance.getCartProducts().reduce((acc, p) => {
        if (productsFromStorage[p.id]) {
            const total = productsFromStorage[p.id].quantity * p.price
            return acc += total;
        }
        return acc;
    }, 0);


    el.innerHTML = `Загальна сумма ${cartTotalSum} ₴`;
}

function onOpenAuthModal() {
    if (CartModal._isShown) {
        CartModal.hide();
    }
    AuthModal.show();
}

function onSort({ target }) {
    const isActive = target.classList.contains('active');
    const apiClient = new ApiClient({ apiBase: API_BASE })


    if (isActive) {
        return;
    }

    const category = target.getAttribute("data-category")
    const field = target.getAttribute("data-field")
    const sort = target.getAttribute("data-sort")

    apiClient.get(`/products?category=${category}&field=${field}&sort=${sort}`).then((response) => {
        const container = document.querySelector(`.products-list-container[data-category="${category}"]`);

        const instance = new ProductsView({
            container: container,
            variant: 'mainView'
        });



        instance.render(response.data.map(p => ({
            id: p.internalId,
            productName: p.productsDetails.productName,
            description: p.productsDetails.description,
            price: p.productsDetails.price,
            image: p.productsDetails.imgURL,
            isActive: p.isActive
        })));
    }).catch(e => {
        console.error(e);
    }).finally(() => {
        const btns = document.querySelectorAll(`.sort-buttons[data-category="${category}"] li button`);

        btns.forEach(b => b.classList.remove('active'));
        target.classList.add('active');
    });
}

function confirmOrder() {
    const apiClient = new ApiClient({ apiBase: API_BASE, isAuth: true })
    const productsFromStorage = JSON.parse(localStorage.getItem('cartProducts'));

    const payload = Object.keys(productsFromStorage).map(id => {
        return {
            productId: id,
            quantity: productsFromStorage[id].quantity
        }
    });

    apiClient.post('/orders', {
        orders: payload
    })
        .then(({ data }) => {
            const totalPrice = document.querySelector('.order-accepted-total');
            const orderId = document.querySelector('.order-accepted-id');

            orderId.innerHTML = `${data.data.id}`
            totalPrice.innerHTML = `${data.data.totalPrice} ₴`
            localStorage.removeItem('cartProducts');
            countCartTotalPrice(true);
            cleaunUpProductCount();

            CartModal.hide();
            OrderAccepted.show();
        })
        .catch(e => {
            console.log(e)

        });
}