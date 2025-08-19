$(document).ready(function () {

    $('#downloadModal').on('shown.bs.modal', function () {
        var $select = $('#ddlTaskModalDownload');

        // Check if select2 is already initialized
        if ($select.hasClass('select2-hidden-accessible')) {
            $select.select2('destroy');
        }

        // Initialize select2
        $select.select2({
            dropdownParent: $(this), // Use the modal as parent
            width: '100%'
        });
    });

    // Clean up on modal hide
    $('#downloadModal').on('hidden.bs.modal', function () {
        $('#ddlTaskModalDownload').select2('destroy');
    });



    $("#TaskId").change(function () {
        var selectedValue = $(this).val();
        var selectedText = $("#TaskId option:selected").text();
        if (selectedValue != null && selectedValue != "") {
            $("#TestCaseName").val(selectedText + "_001");
        }
        else {
            $("#TestCaseName").val("");
        }

      
      

        if (selectedValue != null) {
            $.ajax({
                url: createPartialUrl,
                type: 'GET',
                data: { id: selectedValue, testcaseName: "" },
                success: function (partialViewHtml) {
                   
                    $("#testCaseTableContainer").html(partialViewHtml);
                },
                error: function (xhr, status, error) {
                    console.error("An error occurred: " + error);
                }
            });
        } else {
            console.error("TaskId not found");
        }
    });
});


function setDeleteId(deleteId) {

    toggleBootstrapModal('deleteConfirmationModal', 'show');
    var hiddenInput = document.getElementById("hdnTestcaseIdForDelete");
    if (hiddenInput) {
        hiddenInput.value = deleteId;
    }

   
}
function confirmAndDelete() {
    // Submit the form directly
    document.getElementById("deleteForm").submit();
}
