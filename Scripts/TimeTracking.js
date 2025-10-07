$(document).ready(function () {
    // When modal opens
    $('#createModal').on('shown.bs.modal', function () {
        // Init Select2
        $(this).find('.select2').select2({
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

    //For download modal 
    $('#downloadModal').on('shown.bs.modal', function () {
        const now = new Date();
        const prevMonthFirstDay = new Date(now.getFullYear(), now.getMonth() - 1, 1);

        flatpickr("#downloadStartDate", {
            enableTime: true,
            dateFormat: "d-m-Y",
            time_24hr: true,
            defaultDate: prevMonthFirstDay
        });

        flatpickr("#downloadEndDate", {
            enableTime: true,
            dateFormat: "d-m-Y",
            time_24hr: true,
            defaultDate: now
        });
    });

    //Validation
    $("#btnCreateNewTrack").on("click", function (e) {
        e.preventDefault();

        let form = $(this).closest("form");
        let description = $("#NewRecord_Description").val();
        let descError = $('#descError.invalid-feedback');
        let taskId = $("#NewRecord_TaskId").val();
        let taskError = $("#taskError");

        let isValid = true;

        // Reset errors
        descError.hide();
        taskError.hide();

        // Validate description length
        if (!description || description.trim().length < 10) {
            descError.text('The Description should be at least 10 characters long.').show();
            isValid = false;
        }

        // Validate task selection
        if (!taskId || taskId === "" || taskId === "0") {
            taskError.text('Please select at least one task.').show();
            isValid = false;
        }

        // Submit only if all validations pass
        if (isValid) {
            form.submit();
        }
    });

    //Function for start and stop
    $(document).ready(function () {
        const timers = {}; // store active timers by itemId

        $(".time-track-form").each(function () {
            const $form = $(this);
            const $startBtn = $form.find('button[value="Start"]');
            const $stopBtn = $form.find('button[value="Stop"]');
            const $timerDisplay = $form.find('[id^="Timer_"]');
            const itemId = $timerDisplay.attr("id").split("_")[1];
            let seconds = parseInt($timerDisplay.data("seconds")) || 0;

            // Auto-start timer if status is InProgress (i.e. Start hidden)
            if ($startBtn.hasClass("d-none")) {
                timers[itemId] = startTimer(itemId, $timerDisplay, seconds);
            }

            // --- Start button ---
            $startBtn.on("click", function (e) {
                e.preventDefault();

                // UI toggle
                toggleButtons($startBtn, $stopBtn);

                // Start timer
                timers[itemId] = startTimer(itemId, $timerDisplay, seconds);

                // Update backend
                updateTimeTrack($form, "Start", seconds);
            });

            // --- Stop button ---
            $stopBtn.on("click", function (e) {
                e.preventDefault();

                // UI toggle
                toggleButtons($stopBtn, $startBtn);

                // Stop timer
                clearInterval(timers[itemId]);

                // Update backend
                updateTimeTrack($form, "Stop", seconds);
            });

            // --- Complete / Delete buttons ---
            $form.find('button[value="Complete"], button[value="Delete"]').on("click", function (e) {
                e.preventDefault();
                const action = $(this).val();

                // Stop timer if running
                clearInterval(timers[itemId]);
                toggleButtons($stopBtn, $startBtn);

                // Update backend
                updateTimeTrack($form, action, seconds);
            });

            // ---------- Helper functions ----------
            function startTimer(id, $display, startSeconds) {
                return setInterval(() => {
                    startSeconds++;
                    $display.text(new Date(startSeconds * 1000).toISOString().substr(11, 8));
                }, 1000);
            }

            function toggleButtons($hideBtn, $showBtn) {
                $hideBtn.addClass("d-none");
                $showBtn.removeClass("d-none");
            }
        });

        //Ajax call
        function updateTimeTrack($form, action, seconds) {
            $.ajax({
                type: "POST",
                url: $form.attr("action"), //Identify and reads the action from form /TimeTracking/UpdateTimeTrack
                data: $form.serialize() + "&action=" + action + "&seconds=" + seconds,
                success: function (response) {
                    if (response.success) {
                        // Show success toast
                        showToast("success", response.message || "Action completed successfully.", "Success");

                        // If completed or deleted, remove from view
                        if (response.status === "Completed" || response.deleted) {
                            $form.fadeOut(400, function () { $(this).remove(); });

                            $("#CompletedTaskContainer").load("/TimeTracking/Index #CompletedTaskContainer > *");
                        }
                    } else {
                        // Show warning or error toast depending on server response
                        showToast("warning", response.message || "Update failed. Please retry.", "Warning");
                    }
                },
                error: function (xhr) {
                    showToast("error", "Server error, could not update. (" + xhr.status + ")", "Error");
                }
            });
        }

    });




});

