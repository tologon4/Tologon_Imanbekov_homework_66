// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

function checkUserNameEmail() {
    let value = $('#editInput').val();
    $.ajax({
        type: 'GET',
        url: editCheckUsernameEmail,
        contentType: 'application/json;charset=utf-8',
        dataType: 'json',
        data: { 'value': value },
        success: function (result) {
            if (!result) {
                $('#errorTaken').text(value + ' уже занят!');
            } else {
                $('#errorTaken').text('');
            }
        }
    });
}

function onChange() {
    const password = document.querySelector('input[name=password]');
    const confirm = document.querySelector('input[name=confirm]');
    if (confirm.value === password.value) {
        confirm.setCustomValidity('');
    } else {
        confirm.setCustomValidity('Passwords do not match');
    }
}


$(document).ready(function () {
    $('.edit-button').click(function () {
        let field = $(this).data('field');
        let field2 = $(this).data('field2');
        let currentValue;
        let formId = 'form-' + field;

        if ($('#' + formId).length) {
            $('#' + formId).remove();
            return;
        }

        switch (field) {
            case 'username':
                currentValue = $('#rusername').text();
                break;
            case 'email':
                currentValue = $('#remail').text();
                break;
            case 'phone':
                currentValue = $('#rphone').text();
                break;
            case 'avatar':
                currentValue = $('#profile-avatar').val();
            default:
                currentValue = '';
        }

        let passInputs =
            `<div class="input-group mt-2">
            <input type="password" onChange="onChange()" class="form-control" name="password" placeholder="Password" id="editInput" aria-describedby="button-addon1">
            <button type="button" id="button-addon1" class="btn password-toggle-icon">
              <svg xmlns="http://www.w3.org/2000/svg" width="20px" height="20px" viewBox="0 0 24 24" fill="none">
                <path d="M12 16.01C14.2091 16.01 16 14.2191 16 12.01C16 9.80087 14.2091 8.01001 12 8.01001C9.79086 8.01001 8 9.80087 8 12.01C8 14.2191 9.79086 16.01 12 16.01Z" stroke="#000000" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"/>
                <path d="M2 11.98C8.09 1.31996 15.91 1.32996 22 11.98" stroke="#000000" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"/>
                <path d="M22 12.01C15.91 22.67 8.09 22.66 2 12.01" stroke="#000000" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"/>
              </svg>
            </button>
          </div>
          <div class="input-group mt-2">
            <input type="password" onChange="onChange()" class="form-control" name="confirm" placeholder="Confirm Password" id="editInputConfPass" aria-describedby="button-addon2">
            <button type="button" id="button-addon2" class="btn password-toggle-icon">
              <svg xmlns="http://www.w3.org/2000/svg" width="20px" height="20px" viewBox="0 0 24 24" fill="none">
                <path d="M12 16.01C14.2091 16.01 16 14.2191 16 12.01C16 9.80087 14.2091 8.01001 12 8.01001C9.79086 8.01001 8 9.80087 8 12.01C8 14.2191 9.79086 16.01 12 16.01Z" stroke="#000000" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"/>
                <path d="M2 11.98C8.09 1.31996 15.91 1.32996 22 11.98" stroke="#000000" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"/>
                <path d="M22 12.01C15.91 22.67 8.09 22.66 2 12.01" stroke="#000000" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round"/>
              </svg>
            </button>
          </div>`;

        let formHtml =
            `<form id="${formId}" xmlns="http://www.w3.org/1999/html" enctype="multipart/form-data">
            <div class="form-group" id="editForm">
              <label for="editInput">Новый ${field2}:</label>
              ${field === 'password' ? passInputs :
                `</br><label for="editInput" id="errorTaken" class="text-danger"></label>
                <input onkeyup="checkUserNameEmail()" type="${field === 'email' ? 'email' : field ==='avatar' ? 'file' : 'text'}" class="form-control" id="editInput" value="${currentValue}">`}
            </div>
            <button type="submit" class="btn btn-primary mt-4">Сохранить</button>
            <button type="button" class="btn btn-light mt-4 ms-3" id="cancel-button">Отмена</button>
          </form>`;

        $('#change-div').html(formHtml);

        $('#' + formId).submit(async function(e) {
            e.preventDefault();

            let newValue;
            let isAvatar = (field === 'avatar');
            let formData = new FormData();

            formData.append('key', field);

            if (isAvatar) {
                let file = $('#editInput').prop("files")[0];
                formData.append('uploadedFile', file);
                newValue = file.name;
            } else {
                newValue = $('#editInput').val();
                formData.append('value', newValue);
            }

            try {
                let response = await $.ajax({
                    url: edit,
                    type: 'POST',
                    processData: false,
                    contentType: false,
                    data: formData,
                });
                if (response.result) {
                    if (isAvatar){
                        $('#r' + field).attr('src', response.avatar)
                    }else{
                        $('#r' + field).text(newValue);
                    }
                } else {
                    showError('Невозможно изменить ' + field2);
                }
            } catch (error) {
                console.error('Ошибка загрузки:', error);
                showError('Произошла ошибка при изменении ' + field2);
            } finally {
                hideLoadingIndicator(); 
            }

            alert('Новое значение: ' + newValue);
            $('#change-div').empty();
        });

        function showError(message) {
            $('#editForm').append('<span class="mx-auto text-danger" id="errorEdit">' + message + '</span>');
            setTimeout(function() {
                $('#errorEdit').remove();
            }, 5000);
        }


        $('#cancel-button').click(function (e) {
            e.preventDefault();
            $('#change-div').empty();
        });

        $('#button-addon1').click(function (e) {
            e.preventDefault();
            let inputPass = $('#editInput');
            inputPass.attr('type', inputPass.attr('type') === 'password' ? 'text' : 'password');
        });

        $('#button-addon2').click(function (e) {
            e.preventDefault();
            let inputConfPass = $('#editInputConfPass');
            inputConfPass.attr('type', inputConfPass.attr('type') === 'password' ? 'text' : 'password');
        });
    });
});
