// @ts-nocheck
function loadGrid(o) {
  $(o.el).DataTable(o);

  var filterHead = 1;

  if (typeof o.filterHead !== "undefined") {
    filterHead = +o.filterHead - 1;
  }

  $(o.el + " thead tr:eq(" + filterHead + ") th").each(function(i) {
    var width = "100%";

    if (typeof o.columns[i].width !== "undefined") {
      width = o.columns[i].width + "px";
    }

    if (
      o.columns[i].filter === true ||
      typeof o.columns[i].filter === "undefined"
    ) {
      $(this).html(
        '<input type="text" class="form-control input-sm" style="width: ' +
          width +
          ';" />'
      );
    } else {
      $(this).html(
        '<input type="text" class="form-control input-sm" readonly style="width: ' +
          width +
          ';" />'
      );
    }

    $("input", this).on("keyup change", function(e) {
      if (
        $(o.el)
          .DataTable()
          .column(i)
          .search() !== this.value
      ) {
        if (e.which === 13 || this.value === "") {
          $(o.el)
            .DataTable()
            .column(i)
            .search(this.value)
            .draw();
        }
      }
    });
  });

  return $(o.el).DataTable();
}

function getRowsSelected(selector) {
  var data = [];

  var row_selected = $(selector)
    .DataTable()
    .rows(".selected", { selected: true })
    .data();

  if (typeof row_selected !== "undefined" && row_selected.length > 0) {
    $.each(row_selected, function(i, v) {
      data.push(v);
    });

    return data;
  } else {
    return [];
  }
}

function reloadGrid(selector, resetPage) {
  if (typeof resetPage != "undefined") {
    $(selector)
      .DataTable()
      .ajax.reload(null, false);
  } else {
    $(selector)
      .DataTable()
      .ajax.reload();
  }
}
