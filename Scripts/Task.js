$(document).ready(function () {
    const savedSearchType = localStorage.getItem("searchType");
    const savedSearchString = localStorage.getItem("searchString");

    if (savedSearchType) {
        const selectedOption = $('.search-option[data-search-type="' + savedSearchType + '"]');
        $('#searchTypeButton').text(selectedOption.text());
        $('#hdnSearchType').val(savedSearchType);
    }

    if (savedSearchString) {
        $('#searchString').val(savedSearchString);
    }
});


$('#createTaskForm').on('submit', function (e) {
    if (!validateCreateInput()) {
        e.preventDefault();
    }
})

//shows the modal
function showDeleteModal() {
    toggleBootstrapModal('deleteConfirmationModal', 'show');
}

function validateCreateInput() {
   
    var isValid = true;

    var $errorForTaskname = $('#errmsgMdlTaskName');
    var $newTaskName = $('#NewTask_TaskName');

    var $errorForDescription = $('#errmsgMdlTaskDescription');
    var $newTaskDescription = $('#NewTask_TaskDescription');

    if ($newTaskName.length && ($newTaskName.val().trim() === "" || $newTaskName.val().length < 3)) {
        $errorForTaskname.removeClass('d-none');
        isValid = false; 
    } else {
        $errorForTaskname.addClass('d-none');
    }

    if ($newTaskDescription.length && ($newTaskDescription.val().trim() === "" || $newTaskDescription.val().length < 3)) {
        $errorForDescription.removeClass('d-none');
        isValid = false;
    } else {
        $errorForDescription.addClass('d-none');
    }
    return isValid;
}

//search functionality
$('.search-option').on('click', function (e) {
    e.preventDefault();
    var selectedText = $(this).text();
    var selectedValue = $(this).data('search-type');
    $('#searchTypeButton').text(selectedText);
    $('#hdnSearchType').val(selectedValue);
});

$('#btnSearchTask').on('click', function (e) {
    e.preventDefault();
    const searchType = $('#hdnSearchType').val();
    const searchString = $('#searchString').val();

    localStorage.setItem("searchType", searchType);
    localStorage.setItem("searchString", searchString);

    $('#searchForm').submit();
});

$('#btnRefreshSearch').on('click', function (e) {
    e.preventDefault();
    clearSearchStorage(e);
    $('#searchForm').submit();
});

function clearSearchStorage(e) {
    e.preventDefault();
    localStorage.removeItem("searchType");
    localStorage.removeItem("searchString");
    $('#searchString').val('');
    $('#hdnSearchType').val('');
    $('#hdnSearchType').val('');

    $('#searchForm').submit();
  
}
