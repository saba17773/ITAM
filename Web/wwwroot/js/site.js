// @ts-nocheck
$(document).ready(function() {
  var url = window.location;

  // for sidebar menu entirely but not cover treeview
  $("ul.sidebar-menu a")
    .filter(function() {
      return this.href == url;
    })
    .parent()
    .addClass("active");

  // for treeview
  $("ul.treeview-menu a")
    .filter(function() {
      return this.href == url;
    })
    .parentsUntil(".sidebar-menu > .treeview-menu")
    .addClass("active");

  $(".input-require").append(" <span style='color:red;'>*</span> ");
});

/**
 *
 * @param {*} object
 * data: array
 * id: string
 * value: string
 * defaultSelect: boolean
 * defaultValue: string
 */
function editDropdown(object) {
  var result = [];

  if (object.defaultSelect === true) {
    if (typeof object.defaultValue !== "undefined") {
      result.push({ value: object.defaultValue, text: "" });
    }
  }

  if (object.data.length > 0) {
    $.each(object.data, function(i, v) {
      result.push({ value: v[object.id], text: v[object.value] });
    });
  }

  return result;
}

/**
 *
 * @param {*} object
 * type: string
 * url: string
 * data: object
 */
function ajax(object) {
  if (typeof object.data === "undefined") {
    object.data = {};
  }

  return $.ajax({
    type: object.type,
    url: object.url,
    data: object.data,
    dataType: "json",
    cache: false,
  });
}

/**
 *
 * @param {*} object
 * selector: string
 * data: array
 * id: string
 * value: string
 * defaultValue: string
 */
function generateDropdown(object) {
  try {
    var el = $(object.selector).html("");

    $.each(object.data, function(i, v) {
      el.append(
        "<option value='" + v[object.id] + "'>" + v[object.value] + "</option>"
      );
    });

    if (
      typeof object.defaultValue !== "undefined" &&
      object.defaultValue !== null
    ) {
      el.value(object.defaultValue);
    }
  } catch (err) {
    console.log(err.message);
  }
}

/**
 *
 * @param {*} slug
 * type: string
 */
function slugify(slug) {
  return slug
    .toString()
    .toLowerCase()
    .replace(/\s+/g, "-") // Replace spaces with -
    .replace(/&/g, "-and-") // Replace & with 'and'
    .replace(/[^\w\-]+/g, "") // Remove all non-word characters
    .replace(/\-\-+/g, "-") // Replace multiple - with single -
    .replace(/^-+/, "") // Trim - from start of text
    .replace(/-+$/, ""); // Trim - from end of text
}

/**
 *
 * @param {*} data
 * type: string
 */
function isEmpty(data) {
  if (data === null || data === "") return "";
  return data;
}

/**
 *
 * @param {*} errorArray
 * type: array
 */
function errorMessage(errorArray) {
  var err = "";
  if (errorArray !== "undefined" && errorArray !== null) {
    $.each(errorArray, function(i, v) {
      err += v + "\n";
    });
    return err;
  }
  return "Error : Something wrong!";
}

/**
 *
 * @param {*} object
 * className: string
 * id: int
 * data: array
 * setColor: array
 */
function editable(object) {
  var defaultColor = "";
  if (typeof object.setColor === "undefined") {
    defaultColor = "";
  } else if (object.setColor[0] === object.data) {
    defaultColor = "green";
  } else if (object.setColor[1] === object.data) {
    defaultColor = "red";
  } else if (object.setColor[2] === object.data) {
      defaultColor = "red";
  } else {
    defaultColor = "";
  }

  return (
    '<a href="#" style="color:' +
    defaultColor +
    ';" class="' +
    object.className +
    '" data-pk="' +
    object.id +
    '">' +
    isEmpty(object.data) +
    "</a>"
  );
}
