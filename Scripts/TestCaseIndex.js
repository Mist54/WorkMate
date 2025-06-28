// This file contains all the js codes related to Testcase index page 

//Page load code
$(document).ready(function () {

    buildTestCaseTable();

});

//Event for button click
$('#btnSaveToTable').on('click', function () {
    let formData = [];
    let existingData = JSON.parse(localStorage.getItem('testCaseData')) || [];
    let isValid = true;
    let emptyFields = [];

    // Define input IDs that are allowed to be empty
    let allowedEmptyIds = ['ActualResult', 'Notes']; // Add your optional field IDs here

    $('label[for]').each(function () {
        let labelText = $(this).text().trim();
        let inputId = $(this).attr('for');
        let inputElement = $('#' + inputId);

        // Skip if element doesn't exist or is disabled
        if (!inputElement.length || inputElement.is(':disabled')) {
            return true; // continue to next iteration
        }

        let inputValue = '';
        let displayText = '';

      
        if (inputElement.is('select')) {
           
            inputValue = inputElement.val();
            displayText = inputElement.find('option:selected').text().trim();

            // Check if dropdown has a valid selection (not empty or placeholder)
            // Skip validation if this field is allowed to be empty
            if ((!inputValue || inputValue === '' || inputValue === '0' || displayText === 'Select...' || displayText === '')
                && !allowedEmptyIds.includes(inputId)) {
                isValid = false;
                emptyFields.push(labelText);
                return true; 
            }
        } else {
         
            inputValue = inputElement.val()?.trim() || '';
            displayText = inputValue;

          
            if (inputValue === '' && !allowedEmptyIds.includes(inputId)) {
                isValid = false;
                emptyFields.push(labelText);
                return true;
            }
        }

        // Store the data (even if empty for allowed fields)
        formData.push({
            label: labelText,
            value: inputValue,
            text: displayText
        });
    });

    // Check if validation failed
    if (!isValid) {
        

        showToast("error", `Please fill in the following required fields: ${emptyFields.join(', ')}`);
        return; 
    }

    // Check if any data was collected
    if (formData.length === 0) {
        alert('No valid form data found to save.');
        showToast("error", 'No valid form data found to save.');
        return;
    }

    // Save data if everything is valid
    existingData.push(formData);
    localStorage.setItem('testCaseData', JSON.stringify(existingData));
    buildTestCaseTable();
    clearForm();
});

//builds the Testcase table
function buildTestCaseTable() {
    let storedData = JSON.parse(localStorage.getItem('testCaseData')) || [];

    const specialLabels = ['TestCase Type', 'TestCase Priority', 'TestCase Status'];

    if (storedData.length === 0) {
        $('#TestCaseTable').html('<p class="alert alert-warning">No data available.</p>');
        return;
    }
    let headers = storedData[0].map(item => item.label);
    let tableHtml = '<table class="table table-bordered"><thead><tr>';

    headers.forEach(header => {
        tableHtml += `<th>${header}</th>`;
    });
    tableHtml += '</tr></thead><tbody>';

    storedData.forEach(rowData => {
        tableHtml += `<tr>`;
        rowData.forEach(cell => {
            if (specialLabels.includes(cell.label)) {
                tableHtml += `<td>${cell.text || cell.value}</td>`;
            } else {
                tableHtml += `<td>${cell.value}</td>`;
            }
        });
        tableHtml += '</tr>';
    });


    tableHtml += '</tbody></table>';

    $('#TestCaseTable').html(tableHtml);
}

//Event to clears the local storge
$('#btnClearForm').on('click', function () {

    clearLcoalStorage();
    buildTestCaseTable();

});

//button click event for the process button
$('#btnGenerateTestCase').on('click', function () {

    ProcessFile();

});


//clears the local storge
function clearLcoalStorage() {
    localStorage.removeItem('testCaseData');
}

$('#TaskName').on('focusout', function () {
    let taskName = $(this).val().trim();
    let testCaseNameField = $('#TestCaseName');
    let currentValue = testCaseNameField.val().trim();
    if (currentValue === '' && taskName !== '') {
        testCaseNameField.val(taskName + '_001');
    }
});


//function to call and genrate process files
function ProcessFile() {
    let normalizedData = normalizeData();
    const token = $('input[name="__RequestVerificationToken"]').val();

    $.ajax({
        url: '/TestGenerator/SubmitFile',
        type: 'POST',
        contentType: 'application/json',
        headers: {
            'X-CSRF-Token': token
        },

        data: JSON.stringify({ TestCases: normalizedData }),
        success(response) {
            if (response.success) {
                showToast("success", response.message);
                //Force file download
                window.location.href = response.downloadUrl;
            } else {
                showToast("error", response.message);
            }
        },
        error(err) {
            showToast("error", response.message);
        }
    });
}


//this function used to normalise and convert the raw data of table into object.
function normalizeData() {
    let raw = JSON.parse(localStorage.getItem('testCaseData')) || [];
    return raw.map(entry => {
        const item = { CreatedBy: "" };
        entry.forEach(f => {
            switch (f.label.trim()) {
                case "Task Slno":
                    item.TaskId = parseInt(f.value) || 0;
                    break;
                case "Task name":
                    item.TaskName = f.value;
                    break;
                case "Test case":
                    item.TestCaseName = f.value;
                    break;
                case "Pre condition":
                    item.Preconditions = f.value;
                    break;
                case "Testcase approch steps":
                    item.Steps = f.value;
                    break;
                case "Expected result":
                    item.ExpectedResult = f.value;
                    break;
                case "Actual result":
                    item.ActualResult = f.value;
                    break;
                case "Dev notes":
                    item.Notes = f.value;
                    break;
                case "Testcase type":
                    item.Type = parseInt(f.value);
                    break;
                case "Testcase priority":
                    item.Priority = parseInt(f.value);
                    break;
                case "Testcase satus":
                    item.TestCaseStatus = parseInt(f.value);
                    break;

            }
        });
        return item;
    });
}


function clearForm() {
    $('input, textarea').each(function () {
        let getId = $(this).attr('id');
        const name = $(this).attr('name');

        // Skip the CSRF token by name
        if (name === '__RequestVerificationToken') return;

        if (getId !== 'TaskId' && getId !== 'TaskName' && getId !== 'TestCaseName') {
            $(this).val('');
        }


        if (getId === 'TaskName' || getId === 'TestCaseName') {
            let currentValue = $(this).val().trim();

            if (!currentValue) {
                $(this).val("Task_001");
            } else {
                let match = currentValue.match(/^(.*?)(?:_(\d{3}))?$/);
                if (match) {
                    let prefix = match[1];
                    let number = parseInt(match[2] || "0", 10) + 1;
                    let newValue = prefix + "_" + number.toString().padStart(3, '0');
                    $(this).val(newValue);
                } else {
                    // fallback, if something unexpected
                    $(this).val(currentValue + "_001");
                }
            }
        }


        //set fucous on pre-condition textbox
        $('#Preconditions').focus();
    });


}