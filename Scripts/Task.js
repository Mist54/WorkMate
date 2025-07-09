$(document).ready(function () {
    
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
    var $errorForTaskname = $('#errmsgMdlTaskName');
    var $newTaskName = $('#NewTask_TaskName');

    var $errorForDescription = $('#errmsgMdlTaskDescription');
    var $newTaskDescription = $('#NewTask_TaskDescription');

    if ($errorForTaskname.length && $newTaskName.length) {
        if ($newTaskName.val().trim() === "") {
            $errorForTaskname.removeClass('d-none');
            return false;
        } else {
            $errorForTaskname.addClass('d-none');
        }
    }

    if ($errorForDescription.length && $newTaskDescription.length) {
        if ($newTaskDescription.val().trim() === "") {
            $errorForDescription.removeClass('d-none');
            return false;
        } else {
            $errorForDescription.addClass('d-none');
        }
    }

    return true;
}
