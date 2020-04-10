var utility = {};
var settingsPage = {};

settingsPage.DataTableAdd = function (table, dotNetRef) {
    $(document).ready(function () {
        var dt = $(table).DataTable({
              "serverSide": true,
            "ajax": function (data, callback) {
                dotNetRef.invokeMethodAsync('LoadData', data).then(message => {
                    callback(message);
                    dt.rows('[sel=1]').select();
                });                      
            },
            "columns": [
                { "data": "StockID" },
                { "data": "StockNumber" },
                { "data": "Inventory" },
                { "data": "Description" },
                { "data": "Location" },
                { "data": "Manufacturer" },
                { "data": "Vendor" }
            ],
           
            select: {
                style: 'multi'
            }
        });       
        dt.on('select', function (e, dt, type, indexes) {
            if (type === 'row') {
                var data = dt.rows(indexes).data()[0];
                dotNetRef.invokeMethodAsync('AddStock', data['StockID']);
            }
        });
        dt.on('deselect', function (e, dt, type, indexes) {
            if (type === 'row') {
                var data = dt.rows(indexes).data()[0];
                dotNetRef.invokeMethodAsync('RemoveStock', data['StockID']);
            }
        }

        );

    });
}


utility.DataTablesAdd = function (table) {
    $(document).ready(function () {
        $(table).DataTable();
    });
}
utility.DataTablesRemove = function (table) {
    $(document).ready(function () {
        $(table).DataTable().destroy();
        try {
            var elem = document.querySelector(table + '_wrapper');
            elem.parentNode.removeChild(elem);
        }
        catch (err) { }
    });
}