$(document).ready(function () {
    let $clone = null; // Store the clone element
    let draggedIndex = null; // Store index of dragged element
    let aboveIndex = null; // Track above element
    let belowIndex = null; // Track below element

    $(document).on("dragstart", ".draggable", function (event) {
        let original = $(this);
        draggedIndex = $(".draggable").index(original);
        let offsetX = event.originalEvent.clientX;
        let offsetY = event.originalEvent.clientY;

        // Prevent default drag image
        let img = new Image();
        event.originalEvent.dataTransfer.setDragImage(img, 0, 0);

        $clone = original.clone()
            .removeClass("draggable") // Remove draggable class
            .addClass("bg-opacity-50 shadow-lg")
            .css({
                width: original.outerWidth() + "px",
                height: original.outerHeight() + "px",
                position: "absolute",
                top: offsetY + "px",
                left: offsetX + "px",
                transform: "translate(-50%, -50%)",
                zIndex: 1000,
                pointerEvents: "none",
            });

        $("body").append($clone);
    });


    $(document).on("drag", function (event) {
        if ($clone) {
            $clone.css({
                top: event.originalEvent.clientY + "px",
                left: event.originalEvent.clientX + "px"
            });
        }
        console.log("Clone ", $clone);
    });

    // Track cursor position relative to the container
    $("#details-container").on("mousemove dragover", function (event) {
        let containerOffset = $(this).offset();
        let relativeX = Math.round(event.pageX - containerOffset.left);
        let relativeY = Math.round(event.pageY - containerOffset.top);

        // Find elements above/below
        let elements = $(".draggable");
        aboveIndex = null;
        belowIndex = null;

        elements.each(function (index) {
            let $el = $(this);
            let elOffset = $el.position().top; // Position relative to container
            let elHeight = $el.outerHeight();
            let centerY = elOffset + elHeight / 2; // Center point

            if (relativeY < centerY) {
                belowIndex = index;
                return false; // Exit loop early
            }
            aboveIndex = index;
        });
    });

    $(document).on("dragend", function () {
        console.log("Drag ended");

        if ($clone) {
            $clone.remove();
            $clone = null;
        }

        if (draggedIndex !== null) {
            let targetIndex = null;

            // Determine swap target based on aboveIndex and belowIndex
            if (aboveIndex === null && belowIndex !== null) {
                targetIndex = belowIndex; // Dragging above the first element
            } else if (aboveIndex !== null && belowIndex !== null) {
                targetIndex = (Math.abs(draggedIndex - aboveIndex) < Math.abs(draggedIndex - belowIndex))
                    ? aboveIndex
                    : belowIndex;
            } else if (aboveIndex !== null && belowIndex === null) {
                targetIndex = aboveIndex; // Dragging below the last element
            }

            // Ensure valid swap
            if (targetIndex !== null && targetIndex !== draggedIndex) {
                swapElementsSmoothly(draggedIndex, targetIndex);
            }
        }

        // Reset indices after drag end
        draggedIndex = null;
        aboveIndex = null;
        belowIndex = null;
    });

    function swapElementsSmoothly(index1, index2) {
        const elements = $(".draggable");

        if (index1 < 0 || index2 < 0 || index1 >= elements.length || index2 >= elements.length) {
            console.error("Invalid indices provided.");
            return;
        }

        let $el1 = $(elements[index1]);
        let $el2 = $(elements[index2]);

        let pos1 = $el1.position();
        let pos2 = $el2.position();

        let deltaY = pos2.top - pos1.top;

        $el1.css({ transition: "transform 0.3s ease-in-out", transform: `translateY(${deltaY}px)` });
        $el2.css({ transition: "transform 0.3s ease-in-out", transform: `translateY(${-deltaY}px)` });

        setTimeout(() => {
            $el1.css({ transition: "", transform: "" });
            $el2.css({ transition: "", transform: "" });

            swapElement($el1, $el2);

        }, 300);
    }
});

function swapElement(a, b) {
    // create a temporary marker div
    var aNext = $('<div>').insertAfter(a);
    a.insertAfter(b);
    b.insertBefore(aNext);
    // remove marker div
    aNext.remove();
}
