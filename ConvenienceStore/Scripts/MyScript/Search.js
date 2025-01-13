$().ready(function () {
    const fields = [
        "#skuField2", "#product_name2", "#category_id2", "#quantity2",
        "#description2", "#unit_of_measure2", "#low_stock_threshold2",
        "#expiration_date2", //"#supplier_id2"
    ];

    // Disable all fields and buttons initially
    function disableFields() {
        fields.forEach(field => $(field).prop("disabled", true)); // Use .prop() for better compatibility
        $("#update-btn").prop("disabled", true);
        $("#delete-btn").prop("disabled", true);
    }

    function enableFields() {
        $("#quantity2").prop("disabled", false);
        $("#description2").prop("disabled", false);
        $("#unit_of_measure2").prop("disabled", false);
        $("#low_stock_threshold2").prop("disabled", false);
        $("#expiration_date2").prop("disabled", false);
        $("#supplier_id2").prop("disabled", false);

        $("#update-btn").prop("disabled", false);
        $("#delete-btn").prop("disabled", false);
    }

    // Initialize the page with disabled fields
    disableFields();

    // Handle Search Button Click
    $("#searchBtn").click(function (event) {
        event.preventDefault();
        const sku = $("#searchSku").val();

        if (!sku) {
            alert("Please enter a SKU to search!");
            return;
        }

        $.post("../Home/ProdSearch", { sku: sku })
            .done(function (data) {
                if (data[0].mess === 0) {
                    enableFields();

                    // Populate data into fields
                    $("#skuField2").val(data[0].sku);
                    $("#product_name2").val(data[0].product_name);
                    $("#category_id2").val(data[0].category_id);
                    $("#quantity2").val(data[0].quantity);
                    $("#description2").val(data[0].description);
                    $("#unit_of_measure2").val(data[0].unit_of_measure);
                    $("#low_stock_threshold2").val(data[0].low_stock_threshold);
                    $("#expiration_date2").val(data[0].expiration_date);
                    $("#supplier_id2").val(data[0].supplier_id);
                } else {
                    alert("No Product Found!");
                    disableFields();
                }
            })
            .fail(function () {
                alert("An error occurred while searching for the product.");
            });
    });


    $("#update-btn").click(function (event) {
        event.preventDefault();
        var searchSku = $("#searchSku").val();
        var sku = $("#skuField2").val();
        var name = $("#product_name2").val();
        var category = $("#category_id2").val();
        var quantity = $("#quantity2").val();
        var description = $("#description2").val();
        var unit = $("#unit_of_measure2").val();
        var low = $("#low_stock_threshold2").val();
        var expiration = $("#expiration_date2").val();
        var supplier = $("#supplier_id2").val();

        if (supplier === "" || supplier === null) {
            alert("Please select a supplier.");
            return; // Prevent submission
        }

        $.post("../Home/ProdUpdate", {
            searchSku: searchSku,
            sku: sku,
            name: name,
            category: category,
            quantity: quantity,
            description: description,
            unit: unit,
            low: low,
            expiration: expiration,
            supplier: supplier

        }, function (data) {
            if (data[0].mess == 0) {
                alert("The data was successfully updated");
                location.reload();
            }
        });
    }); 

    $("#delete-btn").click(function (event) {
        event.preventDefault();
        var sku = $("#searchSku").val();
        $.post("../Home/DeleteItem", {
           sku:sku
        }, function (data) {
            if (data[0].mess == 0) {
                alert('Data was successfully removed');
                location.reload();
            }
        });
    });
});
