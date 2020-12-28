const getDownloadButtonStyle = extension => {

  const styles = {
    rdf: {
      backgroundColor: "#3f51b5",
      color: "white"
    },
    html: {backgroundColor: "#00bcd4"},
    csv: {backgroundColor: "#ffeb3b"},
    xml: {backgroundColor: "#ff5722"},
    json: {backgroundColor: "#ff9800"},
    xls: {backgroundColor: "#4caf50"}
  };

  return styles[extension] || {};
};

export default getDownloadButtonStyle;