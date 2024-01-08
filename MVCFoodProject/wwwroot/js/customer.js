document.addEventListener('DOMContentLoaded', () => {
     new EditProfile({
        editor: document.querySelector('.edit-customer-form'),
        openEditor: document.querySelector('.edit-user-profile-btn'),
        modalSelector: '.edit-user-modal',
        onSuccsess: (res) => {
            const editor = document.querySelector('.edit-customer-form');
            const customerAvatar = document.querySelector('.customer-avatar img');
            const customerName = document.querySelector('.customer-name');
            const customerProfileName = document.querySelector('.customer-profile-name');
            const customerPhone = document.querySelector('.customer-phone');
            const customerAddress = document.querySelector('.customer-address');
            
            if (res.data.imgURL) {
                customerAvatar.src = res.data.imgURL;
                editor.querySelector('.image-preview').src = res.data.imgURL;
            }

            if (res.data.name) {
                customerName.innerHTML = res.data.name;
                customerProfileName.innerHTML = res.data.name;
                editor.querySelector('input[name="name"]').value = res.data.name;
            }

            if (res.data.number) {
                customerPhone.innerHTML = res.data.number;
                editor.querySelector('input[name="number"]').value = res.data.number;
            }

            if (res.data.adress) {
                customerAddress.innerHTML = res.data.adress;
                editor.querySelector('input[name="adress"]').value = res.data.adress;
            }
        },
        onFail: (error) => {
            console.log(error);
        }
    })
})


