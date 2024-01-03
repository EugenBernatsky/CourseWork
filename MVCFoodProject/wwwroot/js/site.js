
class Modal {
    constructor(options = {}) {
        this._validateArgs(options);
        this._options = options;
    }

    init() {
        this._modal = this._options.modal;
        this._openButton = this._options.openButton;
        this._onCloseByEsc = this._options.onCloseByEsc;
        this._onCloseByOutClick = this._options.onCloseByOutClick;
        this._onCloseCallback = this._options._onCloseCallback;
        this._onOpenCallback = this._options.onOpenCallback;
        this._onCloseBtns = document.querySelectorAll('.modal-close-btn');

        this._initEvents()
    }

    _initEvents() {
        const self = this;
        if (this._openButton) {
            this._openButton.addEventListener('click', self._onOpenEvent.bind(self))
        }

        self._onCloseBtns.forEach(node => {
            node.addEventListener('click', self._onCloseEvent.bind(self));
        });
    }

    _onCloseEvent() {
        const self = this;

        self._modal.classList.add('modal-fade');
        document.body.style.overflow = 'auto';

        if (self._onCloseCallback && typeof self._onCloseCallback === 'function') {
            self._onCloseCallback();
        }

        if (this._onCloseByEsc) {
            document.removeEventListener('keyup', this._onCloseByEscp.bind(this))
        }
    }

    _onOpenEvent() {
        this._modal.classList.remove('modal-fade');
        document.body.style.overflow = 'hidden';

        // Calc center position for modal
        const modalDialog = this._modal.querySelector('.modal-dialog');
        const rect = modalDialog.getBoundingClientRect();
        const middelPossion = window.innerHeight / 4;
        modalDialog.style.top = `${(window.scrollY - rect.height) + middelPossion}px`

        if (this._onCloseByEsc) {
            document.addEventListener('keyup', this._onCloseByEscp.bind(this));
        }

        if (this._onCloseByOutClick) {
            document.addEventListener('click', this._onOutClick.bind(this))
        }

        if (this._onOpenCallback && typeof this._onOpenCallback === 'function') {
            this._onOpenCallback();
        }
    }

    _onOutClick(event) {
        if (event.target.classList.contains('modal-backdrop')) {
            this._onCloseEvent();
        }
    }

    _onCloseByEscp(event) {
        if (event.code === 'Escape') {
            this._onCloseEvent();
        }
    }

    _validateArgs(options) {
        // Validate reuqired arguments
        if (!('openButton' in options)) {
            console.warn('Missed selector for open event')
        }

        if (!('modal' in options)) {
            throw new Error('Missed modal element')
        }
    }
}

class Steeper {
    constructor(options) {
        this._options = options;
        this._selectedStep = 0;
    }

    init() {
        this._container = this._options.container;

        this._initSteps();
    }

    _initSteps() {
        this._steps = this._container.querySelectorAll('.step');

        if (!this._steps.length) {
            console.warn('No steps added');
            return;
        }

        this._steps.forEach((s, i) => {
            const currentStep = i;
            s.setAttribute('data-step', currentStep);

            const stepButton = s.querySelectorAll('.step-button');

            if (!stepButton.length) {
                console.warn('No steps added');
                return;
            }

            stepButton.forEach(btn => {
                const action = btn.getAttribute('data-action');

                if (action === 'next') {
                    console.log(this._steps[currentStep + 1])
                    const nextStep = this._steps[currentStep + 1] ? currentStep + 1 : -1;

                    btn.setAttribute('data-step-next', nextStep);
                    btn.disabled = nextStep !== -1 ? false : true

                    btn.addEventListener('click', this._onNextStep.bind(this));
                }

                if (action === 'prev') {
                    const prevStep = this._steps[currentStep - 1] ? currentStep - 1 : -1;

                    btn.setAttribute('data-step-prev', prevStep);
                    btn.disabled = prevStep !== -1 ? false : true;

                    btn.addEventListener('click', this._onPrevStep.bind(this));
                }
            });
        });

        this._steps[this._selectedStep].classList.add('step-active');

    }

    _onNextStep(event) {
        const nextStep = event.target.getAttribute('data-step-next');

        this._steps.forEach(s => {
            if (s.getAttribute('data-step') === nextStep) {
                s.classList.add('step-active');
            } else {
                s.classList.remove('step-active');
            }
        })
    }

    _onPrevStep(event) {
        const prevStep = event.target.getAttribute('data-step-prev');

        this._steps.forEach(s => {
            if (s.getAttribute('data-step') === prevStep) {
                s.classList.add('step-active');
            } else {
                s.classList.remove('step-active');
            }
        })
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

    _getTemplate({ id, productName, quantity, image, price, description }) {
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
    }

    render(products) {
        this._products = products;

        this._container.innerHTML = '';
        this._container.innerHTML = this._products.map(p => this._getTemplate.call(this, p)).join(' ');
    }
}

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
            variant: 'cartProduct'
        });
    }

    async fetchProducts() {
        const products = await this._apiClient.get('/getListProducts');
        this._parseProductsByCategory(products);
    }

    async fetchProductsById(ids) {
        const products = await this._apiClient.get(`/getListProductsById?ids=${ids.join(',')}`);

        this._cartProducts = products.map(this._productMapper)
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

document.addEventListener('DOMContentLoaded', function () {

    productViewModelInstance.initViewModels();

    const cartSteeper = new Steeper({
        container: document.querySelector('.cart-steeper')
    })

    const cartModal = new Modal({
        modal: document.querySelector('.modal-cart'),
        openButton: document.querySelector('.cart-button'),
        onCloseByEsc: true,
        onCloseByOutClick: true,
        onOpenCallback: onOpenCartModal
    });

    cartModal.init();
    cartSteeper.init();

    initCartCount();
    tabComponentInit();
});




function initCartCount() {
    const cartProducts = JSON.parse(localStorage.getItem('cartProducts')) || {};

    const productCount = Object.values(cartProducts).reduce((acc, value) => {
        return acc + value.quantity;
    }, 0)

    if (productCount) {
        incrementProductCount(productCount);
    }
}

function tabComponentInit() {
    const tabNavigationBtns = document.querySelectorAll('.tabs-navigation button');
    const tabs = document.querySelectorAll('.tabs .tab');

    const navigationHandler = (e) => {
        const currentButtonFor = e.target.getAttribute('data-toggle');

        tabs.forEach(t => {
            if (t.id === currentButtonFor) {
                t.classList.add('visible')
            } else {
                t.classList.remove('visible')
            }
        });

    };

    tabNavigationBtns.forEach(tb => tb.addEventListener('click', navigationHandler));

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
    const buttons = document.querySelector('.checkout-step .button-group');

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
                buttons.style.display = 'none';
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
    const buttons = document.querySelector('.checkout-step .button-group');
    buttons.style.display = 'none';
    const productsFromStorage = JSON.parse(localStorage.getItem('cartProducts'));

    if (productsFromStorage && !Object.keys(productsFromStorage).length) {
        return;
    }

    productViewModelInstance.fetchProductsById(Object.keys(productsFromStorage)).then(() => {
        const fillteredProducts = productViewModelInstance.getCartProducts().reduce((acc, p) => {
            return acc.concat({
                ...p,
                quantity: productsFromStorage[p.id].quantity
            });

        }, [])

        const viewInstance = productViewModelInstance.getView('cart');

        if (fillteredProducts.length) {
            viewInstance.render(fillteredProducts);
            buttons.style.display = 'block';
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
        el.innerHTML = ` Total price ${0}`;
        return;
    }

    const cartTotalSum = productViewModelInstance.getCartProducts().reduce((acc, p) => {
        if (productsFromStorage[p.id]) {
            const total = productsFromStorage[p.id].quantity * p.price
            return acc += total;
        }
        return acc;
    }, 0);

    console.log(cartTotalSum)

    el.innerHTML = ` Total price ${cartTotalSum}`;

}