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

function isPublishedCheck(data, resumeId) {
    if (data) {
        $('#publicIdent-'+resumeId).text('Опубликован');
        $('#publicIdentBtn-' +resumeId).text('ОТ-публиковать');
    } else {
        $('#publicIdent-'+resumeId).text('Не опубликован');
        $('#publicIdentBtn-'+resumeId).text('Опубликовать');
    }
}

$().ready(function (){


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


    resumeModel.forEach(function (resumeId) {
        let isPublished = $('#isPublished-' + resumeId).val();
        isPublishedCheck(isPublished, resumeId);
    });

    $('.publicResume').click(function (e) {
        e.preventDefault();
        var resumeId = $(this).data('field');
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
    
    let moduleCounter = 0;
    let moduleObjs = [];

    function addModule(module = {}) {
        moduleCounter++;
        const moduleType = module.type || '';
        const startDate = module.startDate || '';
        const endDate = module.endDate || '';
        const organization = module.organization || '';
        const role = module.role || '';
        const responsibilities = module.responsibilities || '';
        const formId = `module-form-${moduleCounter}`;
        let orgName, roleName, responseName;

        switch (moduleType) {
            case 'jobEx':
                orgName = 'Название Компании';
                roleName = 'Должность';
                responseName = 'Обязанности';
                break;
            case 'eduEx':
                orgName = 'Название Курса, Университета';
                roleName = 'Ваша роль';
                responseName = 'Чему вы научились';
                break;
        }

        const resumeFormHtml = `
                    <div class="module mt-4" id="${formId}">
                        <input type="hidden" name="Modules[${moduleCounter}].Type" value="${moduleType}">
                        <div class="row row-cols-2">
                            <div class="col col-6">
                                <label for="Modules_${moduleCounter}__StartDate">Дата начала</label> 
                                <div class="input-group">
                                    <input type="date" class="form-control" name="Modules[${moduleCounter}].StartDate" placeholder="Дата начала" id="Modules_${moduleCounter}__StartDate" value="${startDate}" required>
                                </div>
                            </div>
                            <div class="col col-6">
                                <label for="Modules_${moduleCounter}__EndDate">Дата окончания</label> 
                                <div class="input-group">
                                    <input type="date" class="form-control" name="Modules[${moduleCounter}].EndDate" placeholder="Дата окончания" id="Modules_${moduleCounter}__EndDate" value="${endDate}" required>
                                </div>
                            </div>
                            <div class="col col-6 mt-3">
                                <label for="Modules_${moduleCounter}__Organization">${orgName}</label> 
                                <div class="input-group">
                                    <input type="text" class="form-control" name="Modules[${moduleCounter}].Organization" placeholder="${orgName}" id="Modules_${moduleCounter}__Organization" value="${organization}" required>
                                </div>
                            </div>
                            <div class="col col-6 mt-3">
                                <label for="Modules_${moduleCounter}__Role">${roleName}</label> 
                                <div class="input-group">
                                    <input type="text" class="form-control" name="Modules[${moduleCounter}].Role" placeholder="${roleName}" id="Modules_${moduleCounter}__Role" value="${role}" required>
                                </div>
                            </div>
                            <div class="col col-12 mt-3">
                                <label for="Modules_${moduleCounter}__Response">${responseName}</label>
                                <div class="input-group">
                                    <textarea class="form-control h-100" name="Modules[${moduleCounter}].Response" id="Modules_${moduleCounter}__Response" placeholder="${responseName}" rows="4" required></textarea>
                                </div>
                            </div>
                        </div>
                        <input type="hidden" value="${moduleType}" id="Modules_${moduleCounter}__Identity"/>
                        <button type="button" class="btn btn-light mt-4 ms-3 module-cancel-button">Отмена</button>
                    </div>`;

        $('#module-change-div').append(resumeFormHtml);

        $(`#${formId} .module-cancel-button`).click(function() {
            $(`#${formId}`).remove();
        });

        const moduleObj = {
            'StartedWorking': startDate,
            'EndedWorking': endDate,
            'OrganizationName': organization,
            'Role': role,
            'Responsibilities': responsibilities,
            'Identity': moduleType
        };

        moduleObjs.push(moduleObj);
    }

    $('.add-module-button').click(function(e) {
        e.preventDefault();
        const resumeField = $(this).data('field');
        addModule({ type: resumeField });
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
