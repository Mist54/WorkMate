$(document).ready(function () {

    // When modal opens
    $('#createModal').on('shown.bs.modal', function () {
        // Init Select2
        $(this).find('.select2').select2({
            placeholder: 'Select option',
            dropdownParent: $(this),
            width: '100%'
        });

        // Destroy existing flatpickr instances before reinit
        $(".flatpickr-datetime").each(function () {
            if (this._flatpickr) {
                this._flatpickr.destroy();
            }
        });

        // Init Flatpickr
        flatpickr(".flatpickr-datetime", {
            enableTime: true,
            dateFormat: "Y-m-d H:i",
            time_24hr: true
        });
    });
});
