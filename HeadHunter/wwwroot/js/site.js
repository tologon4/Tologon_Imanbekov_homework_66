

function modalAdder(resumes, vacancyId) {
    let modal = $(`
        <div class="modal fade"> 
            <div class="modal-dialog modal-xl modal-dialog-centered modal-dialog-scrollable"> 
                <div class="modal-content"> 
                    <div class="modal-header"> 
                        <h1 class="modal-title fs-5" id="staticBackdropLabel">VACANCY NAME ${vacancyId}</h1> 
                        <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button> 
                    </div> 
                    <div class="chats modal-body">
            <div id="messages-place" class=" py-5 px-3 msg-page pt-5">
              
              
              
              
            </div>
          </div>
                   
                    <div class=" modal-footer  py-4 px-4 border-top">
                    <div class="col-12">
                                            <div class="row row-cols-2">
                            <div class="col-6">
                                <div class="input-group">
                                    <select class="form-select" id="resumeInput">
                                        <option disabled="disabled" selected="selected">Выберите резюме</option>
                                        ${
        resumes.map(resume =>
            `<option value="${resume.Id}">${resume.Title}</option>`).join('')
    }
                                    </select>
                                    <button type="button" class="btn border border-primary btn-light text-primary resumeOrMessage-send">Отправить</button>
                                </div>
                            </div>
                            <div class="col-6 d-flex align-items-center">
                                <textarea class="form-control" id="messageInput" placeholder="Введите сообщение..." type="text" rows="1"></textarea>
                                <button type="button" class="btn border border-primary btn-light text-primary resumeOrMessage-send">Отправить</button>
                            </div>
                        </div>

</div>
                    </div> 
                </div> 
            </div> 
        </div>`);
    modal.modal('show');
}

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

function isPublishedCheck(data, resumeId) {
    if (data) {
        $('#publicIdent-'+resumeId).text('Опубликован');
        $('#publicIdentBtn-' +resumeId).text('Снять с публикации');
    } else {
        $('#publicIdent-'+resumeId).text('Не опубликован');
        $('#publicIdentBtn-'+resumeId).text('Опубликовать');
    }
}

$().ready(function (){

    $('.reply-btn').click(function() {
        console.log("Reply button clicked");
        let vacancyId = $(this).data('field');
        console.log("Vacancy ID:", vacancyId);
        modalAdder(currentUserResumes, vacancyId);
        function loadMessages() {
            $.ajax({
                url: chatResults,
                type: 'GET',
                data: { 'vacancyId': vacancyId, 'applicantId': currentUserId },
                success: function(data) {
                    console.log('messages loaded');
                    $('#messages-place').html(data);
                }
            });
        }

        loadMessages();

        setInterval(function() {
            loadMessages();
        }, 5000);

        $(document).on('click', '.resumeOrMessage-send', function(e) {
            e.preventDefault();
            console.log('resume btn clicked');
            let resumeId = $('#resumeInput').val();
            let message = $('#messageInput').val();
            $.ajax({
                url: sendMessage,
                type: 'POST',
                data: {
                    vacancyId: vacancyId,
                    resumeId: resumeId,
                    messageContent: message,
                    applicantId: currentUserId
                },
                success: function(response) {
                    if (response) {
                        console.response;
                        alert('Резюме отправлено успешно!');
                    } else {
                        alert('Ошибка при отправке резюме.');
                    }
                }
            });

        });

        $('.modal').on('hidden.bs.modal', function() {
            clearInterval(intervalId);
        });
        
    });
    
    
    
    $('.vacancy-edit-button').click(function () {
        let field = $(this).data('field');
        let field2 = $(this).data('field2');
        let currentValue;
        let formId = 'form-' + field;

        if ($('#' + formId).length) {
            $('#' + formId).remove();
            return;
        }

        switch (field) {
            case 'name':
                currentValue = $('#rname').text();
                break;
            case 'salary':
                currentValue = $('#rsalary').text();
                break;
            case 'demands':
                currentValue = $('#rdemands').text();
                break;
            case 'exFrom':
                currentValue = $('#rexFrom').text();
                break;
            case 'exTo':
                currentValue = $('#rexTo').text();
                break;
            case 'category':
                currentValue = $('#rcategory').text();
                break;
            default:
                currentValue = '';
        }

        let formHtml = `
        <form id="${formId}" xmlns="http://www.w3.org/1999/html" enctype="multipart/form-data">
            <div class="form-group" id="editForm">
                <label for="editInput">Новый ${field2}:</label>
                </br><label for="editInput" id="errorTaken" class="text-danger"></label>
                ${
            field === 'category' ? `
                    <div class="input-group">
                        <select class="form-select" id="editInput" required>
                            <option disabled="disabled" selected="selected">Выберите Категорию</option>
                            ${
                    categories.map(category =>
                        `<option value="${category}" ${category === currentValue ? 'selected' : ''}>${category}</option>`
                    ).join('')
                }
                        </select>
                    </div>` :
                field === 'demands' ?
                    `<textarea class="form-control h-100" id="editInput"  placeholder="${currentValue}" type="text" rows="4"></textarea>`
                    :
                    `<input type="${
                        field === 'exFrom' || field === 'exTo' || field === 'salary' ? 'number' : 'text'
                    }" class="form-control" id="editInput" value="${currentValue}">`
        }
            </div>
            <button type="submit" class="btn btn-primary mt-4">Сохранить</button>
            <button type="button" class="btn btn-light mt-4 ms-3" id="cancel-button">Отмена</button>
        </form>`;

        $('#vacancy-change-div').html(formHtml);

        $('#' + formId).submit(async function(e) {
            e.preventDefault();

            let newValue;
            let formData = new FormData();
            formData.append('key', field);
            formData.append('vacancyId', vacancyId);
            newValue = $('#editInput').val();
            formData.append('value', newValue);

            try {
                let response = await $.ajax({
                    url: editVacancy,
                    type: 'POST',
                    processData: false,
                    contentType: false,
                    data: formData,
                });
                if (response) {
                    $('#r' + field).text(newValue);
                } else {
                    showError('Невозможно изменить ' + field2);
                }
            } catch (error) {
                console.error('Ошибка загрузки:', error);
                showError('Произошла ошибка при изменении ' + field2);
            } finally {
            }

            alert('Новое значение: ' + newValue);
            $('#vacancy-change-div').empty();
        });

        function showError(message) {
            $('#editForm').append('<span class="mx-auto text-danger" id="errorEdit">' + message + '</span>');
            setTimeout(function() {
                $('#errorEdit').remove();
            }, 5000);
        }

        $('#cancel-button').click(function (e) {
            e.preventDefault();
            $('#vacancy-change-div').empty();
        });
    });


    if (resumeModel !== 0){
        resumeModel.forEach(function (resumeId) {
            let isPublished = $('#isPublished-' + resumeId).val();
            isPublishedCheck(isPublished, resumeId);
        });
    }

    $('.publicResume').click(function (e) {
        e.preventDefault();
        let resumeId = $(this).data('field');
        console.log(resumeId);
        $.ajax({
            url: publicUrl,
            type: 'POST',
            data: { 'id': resumeId },
            success: function (data) {
                isPublishedCheck(data, resumeId);
            }
        });
    });
    $('.updateResume').click(function (e){
        e.preventDefault();
        let resumeId = $(this).data('field');
        console.log(resumeId);
        $.ajax({
            url: updateUrl,
            type: 'POST',
            data: { 'id':  resumeId},
            success: function (data){
                $('#editTime-'+resumeId).text('Updated Time: '+data);
            }
        });
    });
    
    $('.deleteResume').click(function (e){
        e.preventDefault();
        let resumeId = $(this).data('field');
        console.log(resumeId);
        $.ajax({
            url: deleteUrl,
            type: 'POST',
            data: { 'id':  resumeId},
            success: function (data){
                if (data){
                    $('.deleteResume[data-field="' + resumeId + '"]').closest('.resume').remove();
                }
                else {
                    $('.deleteResume[data-field="' + resumeId + '"]').append(`<span class="mx-auto text-danger" id="errorDelete">Не удалось удалить!</span>`);
                    setTimeout(function (){
                        $('#errorDelete').remove();
                    }, 5000)
                }
            }
        });
    });
    
   

    $('#main-form').submit(function() {
        const moduleForms = $('.module');
        moduleObjs = [];

        moduleForms.each(function() {
            const form = $(this);
            const moduleObj = {
                'Identity': form.find('input[name*="Type"]').val(),
                'StartDate': form.find('input[name*="StartDate"]').val(),
                'EndDate': form.find('input[name*="EndDate"]').val(),
                'Organization': form.find('input[name*="Organization"]').val(),
                'Role': form.find('input[name*="Role"]').val(),
                'Response': form.find('textarea[name*="Response"]').val(),
            };

            moduleObjs.push(moduleObj);
        });

        $('#modulesObjs').val(JSON.stringify(moduleObjs));
    });
    
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
                break;
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
