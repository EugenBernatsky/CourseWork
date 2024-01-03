document.addEventListener("DOMContentLoaded", function () {
    const apiCLientInstance = new ApiClient({
        apiBase: API_BASE
    });

    const loginForm = document.querySelector('.login-form');
    const registerForm = document.querySelector('.registration-form');

    const origin = window.location.origin;

    const DESTINATION_MAP_BY_ROLE = {
        admin: 'adminpage',
        customer: 'foodpage',
        courier: 'courierpage',
        unknown: 'foodpage'
    }

    loginForm.addEventListener('submit', function (e) {
        e.preventDefault();
        const data = new FormData(loginForm);

        const payload = {
            username: data.get('name'),
            password: data.get('password'),
           
        }


        apiCLientInstance.post('/login', {
            body: payload
        })
            .then(user => {
                USER_INSTANCE.setUserProfile(user);
                window.location.replace(`${origin}/${DESTINATION_MAP_BY_ROLE[user.role]}`);
            })
            .catch(e => {
                console.error(e)
            });
    });


    registerForm.addEventListener('submit', function (e) {
        e.preventDefault();
        const data = new FormData(registerForm);

        const payload = {
            username: data.get('name'),
            password: data.get('password'),
            email: data.get('email'),
            phone: data.get('phone')
        }

        
        apiCLientInstance.post('/register', {
            body: payload
        })
            .then(user => {
                USER_INSTANCE.setUserProfile(user);
                window.location.replace(`${origin}/${DESTINATION_MAP_BY_ROLE[user.role]}`)
            })
            .catch(e => {
                console.error(e)
            });
    });



});