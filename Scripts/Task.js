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

    // Initialize selects and datepickers on page load
    initSelect2();
    initDatePickers();
});

// Persist search settings when user chooses an option
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
});

// Create form submit validation
$('#createTaskForm').on('submit', function (e) {
    if (!validateCreateInput()) {
        e.preventDefault();
        // keep modal open when validation fails
        toggleBootstrapModal('createTaskModal', 'show');
    }
});

// Selected task (edit) form validation
$('#saveChangesForm').on('submit', function (e) {
    if (!validateSelectedTaskDates()) {
        e.preventDefault();
        // keep user on the same panel; show inline message
        if ($('#SelectedTask_DateError').length === 0) {
            $('<p id="SelectedTask_DateError" class="text-danger small mt-1"><b>End Date must be the same as or after Start Date.</b></p>')
                .insertAfter($('#SelectedTask_EndDate'));
        }
    } else {
        $('#SelectedTask_DateError').remove();
    }
});

// Delete modal helper
function showDeleteModal() {
    toggleBootstrapModal('deleteConfirmationModal', 'show');
}

function clearSearchStorage(e) {
    e && e.preventDefault();
    localStorage.removeItem("searchType");
    localStorage.removeItem("searchString");
    $('#searchString').val('');
    $('#hdnSearchType').val('');
    $('#searchForm').submit();
}

// Validation for create modal (task name, description, dates)
function validateCreateInput() {
    var isValid = true;

    var $errorForTaskname = $('#errmsgMdlTaskName');
    var $newTaskName = $('#NewTask_TaskName');

    var $errorForDescription = $('#errmsgMdlTaskDescription');
    var $newTaskDescription = $('#NewTask_TaskDescription');

    // Remove any previous date error
    $('#errmsgMdlTaskDates').remove();

    if ($newTaskName.length && ($newTaskName.val().trim() === "" || $newTaskName.val().trim().length < 3)) {
        $errorForTaskname.removeClass('d-none');
        isValid = false;
    } else {
        $errorForTaskname.addClass('d-none');
    }

    if ($newTaskDescription.length && ($newTaskDescription.val().trim() === "" || $newTaskDescription.val().trim().length < 3)) {
        $errorForDescription.removeClass('d-none');
        isValid = false;
    } else {
        $errorForDescription.addClass('d-none');
    }

    // Validate Start / End dates if provided
    var startVal = $('#NewStartDate').val();
    var endVal = $('#NewEndDate').val();

    if (startVal && endVal) {
        var startDate = parseDateOnly(startVal);
        var endDate = parseDateOnly(endVal);

        if (startDate && endDate && endDate < startDate) {
            // append a visible inline error under EndDate if not already present
            if ($('#errmsgMdlTaskDates').length === 0) {
                $('<p id="errmsgMdlTaskDates" class="text-danger small mt-1"><b>End Date must be the same as or after Start Date.</b></p>')
                    .insertAfter($('#NewEndDate'));
            }
            isValid = false;
        }
    }

    return isValid;
}

// Validate SelectedTask date ordering
function validateSelectedTaskDates() {
    var startVal = $('#SelectedTask_StartDate').val();
    var endVal = $('#SelectedTask_EndDate').val();

    if (startVal && endVal) {
        var startDate = parseDateOnly(startVal);
        var endDate = parseDateOnly(endVal);

        if (startDate && endDate && endDate < startDate) {
            return false;
        }
    }
    return true;
}

// Parse date string in YYYY-MM-DD or fallback formats safely
function parseDateOnly(val) {
    if (!val) return null;
    var iso = val.trim();
    if (/^\d{4}-\d{1,2}-\d{1,2}$/.test(iso)) {
        var parts = iso.split('-');
        return new Date(parts[0], parts[1] - 1, parts[2]);
    }
    var parsed = new Date(val);
    return isNaN(parsed.getTime()) ? null : parsed;
}

// Initialize Select2 for any .select2 elements
function initSelect2() {
    if ($.fn.select2) {
        $('.select2').each(function () {
            var $el = $(this);
            if ($el.hasClass('select2-hidden-accessible')) return;
            var $modal = $el.closest('.modal');
            var options = {
                width: '100%',
                placeholder: $el.data('placeholder') || '-- Select --',
                allowClear: true
            };
            if ($modal.length) {
                options.dropdownParent = $modal;
            }
            $el.select2(options);
        });
    }
}

// Initialize datepickers for elements with .flatpickr class
function initDatePickers() {
    if (typeof flatpickr !== 'undefined') {
        $('.flatpickr').each(function () {
            var $el = $(this);
            if ($el.data('flatpickr')) return;
            flatpickr(this, {
                dateFormat: 'Y-m-d',
                allowInput: true,
                wrap: false
            });
        });
    } else if ($.fn.flatdatepicker) {
        try {
            $('.flatpickr').flatdatepicker();
        } catch (e) {
            console.warn('flatdatepicker init failed', e);
            $('.flatpickr').attr('type', 'date');
        }
    } else {
        $('.flatpickr').attr('type', 'date');
    }
}

// Re-init widgets when modal shown
$(document).on('shown.bs.modal', '#createTaskModal', function () {
    //initSelect2();
    //initDatePickers();
    //Assigns the parent setup
    $('#NewTask_AssignedToUserId').select2({
        dropdownParent: $('#createTaskModal')
    });
    $('#NewTask_AssignedByUserId').select2({
        dropdownParent: $('#createTaskModal')
    });
});
