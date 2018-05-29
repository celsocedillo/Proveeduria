var tabItems

var Solicitud = function () {
    return {
        init: function () {
            tabItems = $('#tabItems').DataTable({
                "title": "Items por Bodega",
                "paging": false,
                "autoWidth": false
            });
        }
    };
}();