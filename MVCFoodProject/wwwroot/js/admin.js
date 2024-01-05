const apiCLientInstance = new ApiClient({
    apiBase: API_BASE,
    isAuth: true
});

class ProductEditor {
    constructor(options = {}) {
        this._init(options)
    }

    onSubmit() {
        const form = new FormData();

        if (this._tmpProductPrice && Number(this._tmpProductPrice) !== Number(this._price)) {
            form.append('price', Number(this._tmpProductPrice));
        }

        if (this._tmpImg && this._tmpImg !== this._image) {
            form.append('image', this._tmpImg);
        }

        if (this._tmpProductName && this._tmpProductName !== this._productName) {
            form.append('productName', this._tmpProductName);
        }

        if (this._tmpProductDescription && this._tmpProductDescription !== this._description) {
            form.append('description', this._tmpProductDescription);
        }

        if (form.entries().next().value) {
            return apiCLientInstance.post(`/products/product/edit/${this._id}`, form)
        }

        return Promise.resolve();
    }

    _init(options) {
        this._editorContainer = options.editorContainer;
        this._id = options.id;
        this._renderLoader();

        this._tmpProductPrice = null;
        this._tmpProductDescription = null;
        this._tmpProductName = null;
        this._tmpImg = null;

        this._fetchProduct()
            .then(response => {
                this._desirialize(response.data);
                this._initEditor();
                this._initEvents();
            })
    }

    _desirialize(DTO) {
        this._price = DTO.productsDetails.price;
        this._description = DTO.productsDetails.description;
        this._productName = DTO.productsDetails.productName;
        this._image = DTO.productsDetails.imgURL
    }

    _initEvents() {
        const uploaderSelector = document.querySelector('#uplod-image');
        const nameSelector = document.querySelector('#edit-product-name');
        const descriptionSelector = document.querySelector('#edit-product-description');
        const priceSelector = document.querySelector('#edit-product-price');


        uploaderSelector.addEventListener('change', this._imageUploader.bind(this));

        nameSelector.addEventListener('input', this._onInputProductName.bind(this))
        descriptionSelector.addEventListener('input', this._onInputDescription.bind(this))
        priceSelector.addEventListener('input', this._onInputPrice.bind(this))
    }

    _onInputPrice(event) {
        const value = event.target.value;

        this._tmpProductPrice = value
    }

    _onInputDescription(event) {
        const value = event.target.value;

        this._tmpProductDescription = value;
    }

    _onInputProductName(event) {
        const value = event.target.value;

        this._tmpProductName = value
    }

    _imageUploader(event) {
        const image = document.querySelector('.upload-img-preview');

        if (!event.target.files[0] || !image) {
            return;
        }
       
        this._tmpImg = event.target.files[0];
        image.onload = () => {
            URL.revokeObjectURL(image.src);
        }

        image.src = URL.createObjectURL(this._tmpImg);
    }

    _renderLoader() {
        this._editorContainer.innerHTML = `
            <div class="d-flex justify-content-center">
               <div class="spinner-border text-primary" role="status">
                <span class="visually-hidden">Loading...</span>
              </div>
            </div>
        `;
    }

    _initEditor() {
        this._editorContainer.innerHTML = `
            <div class="mb-3">
                    <label for="uplod-image" class="form-label">Завантажити нову картинку</label>
                    <input type="file" accept="image/jpeg, image/png, image/jpg" class="form-control" id="uplod-image" placeholder="">
                    <div class="image-preview d-flex justify-content-center mt-2">
                        <img src="${this._image}" class="img-thumbnail upload-img-preview" alt="...">
                    </div>
                </div>
                <div class="mb-3">
                    <label for="edit-product-name" class="form-label">Редагувати назву продукта</label>
                    <input type="text" class="form-control" id="edit-product-name" placeholder="Редагувати назву продукта" value="${this._productName}">
                </div>
                <div class="mb-3">
                    <label for="edit-product-description" class="form-label">Редагувати опис продукта</label>
                    <input type="text" class="form-control" id="edit-product-description" placeholder="Редагувати опис продукта" value="${this._description}">
                </div>

                <div class="mb-3">
                    <label for="edit-product-price" class="form-label">Редагувати ціну</label>
                    <input type="number" class="form-control" id="edit-product-price" placeholder="Редагувати ціну" value="${this._price}">
                </div>
        
        `
    }

    _fetchProduct() {
        return apiCLientInstance.get(`/products/product/${this._id}`)
    }
}


function editProductHandler(event) {
    const EditModal = new bootstrap.Modal('#edit-product-modal', {
        keyboard: false
    });
    EditModal.show();
    const modalBody = document.querySelector('.modal-body');
    const productId = event.target.getAttribute('data-product-id');
    const submitButton = document.querySelector('.edit-submit');
    const cancelButtons = document.querySelectorAll('.edit-cancel');

    const instance = new ProductEditor({
        editorContainer: modalBody,
        id: productId
    });

    submitButton.addEventListener('click', () => {
        submitButton.setAttribute("disabled", true);
        cancelButtons.forEach(c => c.setAttribute("disabled", true));

        instance.onSubmit().then((result) => {
            if (!result) return;

            const { data: res } = result;
            const product = document.querySelector(`[data-product-id="${res.internalId}"]`);
            const instance = new ProductsView({
                container: product,
                variant: 'adminProductCard'
            });

            instance.render([{
                id: res.internalId,
                productName: res.productsDetails.productName,
                description: res.productsDetails.description,
                price: res.productsDetails.price,
                image: res.productsDetails.imgURL,
                isActive: res.isActive
            }])
        }).catch(e => {
            console.error(e);
        }).finally(() => {
            EditModal.hide();
            submitButton.removeAttribute("disabled");
            cancelButtons.forEach(c => c.removeAttribute("disabled"));
        });
    })
}

function onToggleProduct(event) {
    const payload = {
        internalId: event.target.getAttribute('data-product-id'),
        action: event.target.getAttribute('data-action')
    };

    apiCLientInstance.post(`/products/toggle`, payload).then(({ data: res }) => {
        const product = document.querySelector(`[data-product-id="${res.internalId}"]`);
        const instance = new ProductsView({
            container: product,
            variant: 'adminProductCard'
        });

        instance.render([{
            id: res.internalId,
            productName: res.productsDetails.productName,
            description: res.productsDetails.description,
            price: res.productsDetails.price,
            image: res.productsDetails.imgURL,
            isActive: res.isActive
        }])
    }).catch(e => {
        console.error(e)
    });
}

function onSort({ target }) {
    const isActive = target.classList.contains('active');

    if (isActive) {
        return;
    }

    const category = target.getAttribute("data-category")
    const field = target.getAttribute("data-field")
    const sort = target.getAttribute("data-sort")

    apiCLientInstance.get(`/products?category=${category}&field=${field}&sort=${sort}`).then((response) => {
        const container = document.querySelector(`.products-list-container[data-category="${category}"]`);

        const instance = new ProductsView({
            container: container,
            variant: 'adminMainView'
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

function onDeleteProduct({ target }) {
    const productId = target.getAttribute('data-product-id');

    apiCLientInstance.delete(`/products/${productId}`)
        .then(() => {
            document.querySelector(`.card[data-product-id="${productId}"]`).parentNode.remove();
        }).catch(e => {
            console.error(e)
        })
}