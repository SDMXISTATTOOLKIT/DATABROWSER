import React, {Fragment, useEffect, useState} from "react";
import {compose} from "redux";
import {connect} from "react-redux";
import TreeItem from "@material-ui/lab/TreeItem";
import Checkbox from "@material-ui/core/Checkbox";
import ExpandMoreIcon from "@material-ui/icons/ExpandMore";
import ChevronRightIcon from "@material-ui/icons/ChevronRight";
import TreeView from "@material-ui/lab/TreeView";
import withStyles from "@material-ui/core/styles/withStyles";
import Grid from "@material-ui/core/Grid";
import {Tooltip} from "@material-ui/core";
import IconButton from "@material-ui/core/IconButton";
import LibraryAddCheckIcon from "@material-ui/icons/LibraryAddCheck";
import FilterNoneIcon from "@material-ui/icons/FilterNone";
import HeightIcon from "@material-ui/icons/Height";
import VerticalAlignCenterIcon from "@material-ui/icons/VerticalAlignCenter";
import AutoSearchInput from "../auto-search-input";
import Divider from "@material-ui/core/Divider";
import CustomEmpty from "../custom-empty";
import {getMaxTreeDepth, getNodesAtDepth} from "../../utils/tree";
import _ from "lodash"
import {v4 as uuidv4} from "uuid";
import {useTranslation} from "react-i18next";
import {localizeI18nObj} from "../../utils/i18n";

const $ = window.jQuery;

const TREE_PAGE_SIZE = 10;

const TREE_NODE_KEY_OF_LEVEL_PREFIX = "TREE_NODE_KEY_OF_LEVEL_PREFIX_";

const styles = theme => ({
  root: {
    height: "100%"
  },
  tree: {
    overflow: "auto",
    "& .MuiTreeItem-label": {
      width: "calc(100% - 19px)"
    }
  },
  treeActions: {
    marginBottom: 6
  },
  treeNode: {
    height: 34,
    padding: "2px 0"
  },
  treeNodeCheckbox: {
    display: "inline-block",
    verticalAlign: "middle",
    padding: "4px 8px"
  },
  treeNodeLabel: {
    display: "inline-block",
    verticalAlign: "middle",
    fontSize: 14,
    color: "rgba(0, 0, 0, 0.65)",
    whiteSpace: "nowrap",
    overflow: "hidden",
    textOverflow: "ellipsis"
  },
  levelTreeNodeLabel: {
    color: "rgba(0, 0, 0, 0.85)",
    fontWeight: 500
  },
  treeNodeAction: {
    display: "inline-block",
    verticalAlign: "middle",
    marginLeft: 4,
    "& > button": {
      padding: 4
    }
  },
  divider: {
    margin: "4px 0"
  },
  showMoreNode: {
    marginLeft: 10,
    "& i": {
      fontSize: 14
    }
  },
  treeAction: {
    padding: 8
  }
});

const getKeys = (tree, childrenKey, getNodeKey, test) => {

  const res = [];

  const recursive = subTree =>
    subTree
      ? subTree.map(node => {
        if (node[childrenKey] && node[childrenKey].length) {
          recursive(node[childrenKey]);
        }
        if (!test || test(node)) {
          res.push(getNodeKey(node));
        }
        return null;
      })
      : [];

  recursive(tree);

  return res;
}

const getFilteredTree = (tree, childrenKey, labelKey, searchText, defaultLanguage, languages, isLabelNotMultilingual) => {

  const filterNode = node => {
    const search = searchText.toLowerCase();
    return (
      (node.id && node.id.toLowerCase().indexOf(search) >= 0) ||
      (isLabelNotMultilingual && node[labelKey].toLowerCase().indexOf(search) >= 0) ||
      (!isLabelNotMultilingual && localizeI18nObj(node[labelKey], defaultLanguage, languages).toLowerCase().indexOf(search) >= 0)
    );
  };

  const recursive = subtree => subtree
    ? ([
      ...subtree
        .map(node => ({...node}))
        .filter(node => {
          if (node[childrenKey] && node[childrenKey].length) {
            if (filterNode(node)) {
              return true;
            } else {
              const filteredChildren = [...recursive(node[childrenKey], filterNode)];
              if (filteredChildren.length) {
                node[childrenKey] = filteredChildren;
                return true;
              } else {
                return false;
              }
            }
          } else {
            return filterNode(node);
          }
        })
    ])
    : null;

  return recursive(tree, filterNode);
}

const handleHeight = (uuid, keys, withLevelSelector) => {

  const actionHeight = $(`.enhanced-tree__${uuid} .enhanced-tree__actions`).css("height");

  let levelSelectorHeight = 0;
  if (withLevelSelector) {
    levelSelectorHeight = 32 + 16;
    let found = false;
    for (let i = 0; i < keys.length; i++) {
      if (keys.find(key => key === TREE_NODE_KEY_OF_LEVEL_PREFIX + i) && !found) {
        levelSelectorHeight += 32;
      } else {
        found = true
      }
    }
  }

  $(`.enhanced-tree__${uuid} .enhanced-tree__tree`).css("height", `calc(100% - ${actionHeight} - ${levelSelectorHeight}px)`);
}

function EnhancedTree(props) {

  const {
    languages,
    defaultLanguage,
    classes,
    tree,
    getNodeKey,
    childrenKey,
    labelKey,
    getNodeLabel,
    checkable,
    isCheckDisabled,
    defaultExpandedKeys,
    nodeActions,
    withoutSearch,
    withoutCheckControls,
    withoutExpandControls,
    withLevelSelector,
    withoutPagination,
    getIsCheckable,
    isLabelNotMultilingual
  } = props;

  const {t} = useTranslation();

  const [uuid] = useState(uuidv4());

  const [searchText, setSearchText] = useState("");

  const [checkedKeys, setCheckedKeys] = useState(props.checkedKeys || []);
  const [expandedKeys, setExpandedKeys] = useState(defaultExpandedKeys || []);
  const [showMoreKeyClicks, setShowMoreKeyClicks] = useState({});

  useEffect(() => {
    handleHeight(uuid, [], withLevelSelector)
  }, [uuid, withLevelSelector])

  const handleCheck = (checkedNode, checked) => {

    const newCheckedKeys = [...(props.checkedKeys || checkedKeys)];
    checked
      ? newCheckedKeys.push(getNodeKey(checkedNode))
      : newCheckedKeys.splice((props.checkedKeys || checkedKeys).indexOf(getNodeKey(checkedNode)), 1);

    setCheckedKeys(newCheckedKeys);
    if (props.onCheck) {
      props.onCheck(newCheckedKeys)
    }
  }

  const handleLevelCheck = (checkedNode, checked) => {

    const nodesAtDepth = getNodesAtDepth(filteredTree, childrenKey, checkedNode.level + 1)
      .filter(node => getIsCheckable ? getIsCheckable(node) : true)
      .map(node => getNodeKey(node));

    let newCheckedKeys = [...(props.checkedKeys || checkedKeys)];
    newCheckedKeys = checked
      ? _.uniq(newCheckedKeys.concat(nodesAtDepth))
      : newCheckedKeys.filter(key => !nodesAtDepth.includes(key));

    setCheckedKeys(newCheckedKeys)
    if (props.onCheck) {
      props.onCheck(newCheckedKeys)
    }
  }

  const handleExpand = expandedKeys => {
    setExpandedKeys(expandedKeys);
    if (props.onExpand) {
      props.onExpand(expandedKeys)
    }
  }

  const handleSearch = searchText => {
    setSearchText(searchText)
    setExpandedKeys((searchText && searchText.length > 0)
      ? getKeys(filteredTree, childrenKey, getNodeKey)
      : [] // TODO: quale dovrebbe essere il comportamento corretto?
    )
  }

  const getShowMoreNode = parent => {

    const key = `${parent ? getNodeKey(parent) : 'root'}`;

    const remaining = (parent ? parent[childrenKey] : filteredTree.filter(node => node != null)).length - ((showMoreKeyClicks[key] || 0) + 1) * TREE_PAGE_SIZE;

    return (
      <TreeItem
        key={`${key}_showMore`}
        nodeId={`${key}_showMore`}
        className={classes.showMoreNode}
        label={
          <i
            onClick={e => {
              e.stopPropagation();
              setShowMoreKeyClicks({
                ...showMoreKeyClicks,
                [key]:
                  showMoreKeyClicks[key]
                    ? showMoreKeyClicks[key] + 1
                    : 1
              })
            }}
          >
            {t("components.enhancedTree.showMore", {more: Math.min(remaining, TREE_PAGE_SIZE), remaining: remaining})}
          </i>
        }
      />
    );
  }

  const getTreeNode = (node, getNodeKey, isLevelTree, checkable, checkedKeys, onCheck, getIsCheckable) => {

    const actions = (nodeActions || [])
      .filter(act => act)
      .map(act => typeof (act) === "function" ? act(node) : act)
      .filter(act => act);

    const label = (getNodeLabel && !isLevelTree)
      ? getNodeLabel(node)
      : isLabelNotMultilingual
        ? node[labelKey]
        : localizeI18nObj(node[labelKey], defaultLanguage, languages);

    return (
      <TreeItem
        key={getNodeKey(node)}
        nodeId={getNodeKey(node)}
        label={
          <div className={classes.treeNode}>
            {checkable && (
              <Checkbox
                className={classes.treeNodeCheckbox}
                checked={!isLevelTree
                  ? checkedKeys.includes(getNodeKey(node))
                  : !getNodesAtDepth(filteredTree, childrenKey, node.level + 1).filter(node => getIsCheckable ? getIsCheckable(node) : true).map(node => getNodeKey(node)).some(val => checkedKeys.indexOf(val) === -1)}
                onClick={event => {
                  onCheck(node, event.target.checked);
                  event.stopPropagation();
                }}
                disabled={isCheckDisabled || (getIsCheckable ? !getIsCheckable(node) : false)}
              />
            )}
            <Tooltip title={label}>
              <div
                className={`${classes.treeNodeLabel} ${isLevelTree ? classes.levelTreeNodeLabel : ""}`}
                style={{width: `calc(100% - ${(checkable ? 40 : 0) + ((actions && actions.length > 0) ? 4 + (32 * actions.length) : 0)}px)`}}
              >
                {label}
              </div>
            </Tooltip>
            {!isLevelTree && (
              <div className={classes.treeNodeAction}>
                {actions
                  .filter(action => (action !== null && action !== undefined))
                  .map((action, idx) =>
                    <Tooltip key={idx} title={action.title}>
                      <IconButton
                        onClick={event => {
                          action.onClick(node);
                          event.stopPropagation();
                        }}
                      >
                        {action.icon}
                      </IconButton>
                    </Tooltip>
                  )}
              </div>
            )}
          </div>
        }
      >
        {node[childrenKey]
          ? node[childrenKey]
            .filter((child, index) => withoutPagination || index < TREE_PAGE_SIZE * ((showMoreKeyClicks[getNodeKey(node)] || 0) + 1))
            .map(child => getTreeNode(child, getNodeKey, isLevelTree, checkable, checkedKeys, onCheck, getIsCheckable))
            .concat(!withoutPagination && node[childrenKey].length > TREE_PAGE_SIZE * ((showMoreKeyClicks[getNodeKey(node)] || 0) + 1)
              ? [getShowMoreNode(node)]
              : []
            )
          : null
        }
      </TreeItem>
    )
  };

  const filteredTree = getFilteredTree(tree, childrenKey, labelKey, searchText, defaultLanguage, languages, isLabelNotMultilingual);

  let levelTree = []
  if (withLevelSelector) {
    const maxTreeDepth = getMaxTreeDepth(filteredTree, childrenKey);
    levelTree = [{
      id: TREE_NODE_KEY_OF_LEVEL_PREFIX + 0,
      [labelKey]: isLabelNotMultilingual
        ? t("components.enhancedTree.levelSelector", {level: 0})
        : {
          [defaultLanguage]: t("components.enhancedTree.levelSelector", {level: 0})
        },
      level: 0,
    }];
    if (maxTreeDepth !== null && maxTreeDepth !== 0) {
      [...Array(maxTreeDepth - 1)].forEach(() => {
        let pointer = levelTree[0];
        let counter = 0;
        while (pointer[childrenKey]) {
          pointer = pointer[childrenKey][0];
          counter++;
        }
        pointer[childrenKey] = [
          {
            id: TREE_NODE_KEY_OF_LEVEL_PREFIX + Number(counter + 1),
            [labelKey]: isLabelNotMultilingual
              ? t("components.enhancedTree.levelSelector", {level: counter + 1})
              : {
                [defaultLanguage]: t("components.enhancedTree.levelSelector", {level: counter + 1})
              },
            level: counter + 1
          }
        ]
      });
    }
  }

  return (
    <div className={`enhanced-tree enhanced-tree__${uuid} ${classes.root}`}>
      <div className={`enhanced-tree__actions ${classes.treeActions}`}>
        <Grid container justify="space-between">
          <Grid item xs={6}>
            {!withoutCheckControls
              ? (
                <Fragment>
                  <Tooltip title={t("components.enhancedTree.selectAll.tooltip")}>
                    <span>
                      <IconButton
                        onClick={() => {
                          const newCheckedKeys = getKeys(filteredTree, childrenKey, getNodeKey, getIsCheckable);
                          setCheckedKeys(newCheckedKeys)
                          if (props.onCheck) {
                            props.onCheck(newCheckedKeys)
                          }
                        }}
                        className={classes.treeAction}
                        disabled={isCheckDisabled}
                      >
                        <LibraryAddCheckIcon/>
                      </IconButton>
                    </span>
                  </Tooltip>
                  <Tooltip title={t("components.enhancedTree.deselectAll.tooltip")}>
                    <span>
                      <IconButton
                        onClick={() => {
                          setCheckedKeys([])
                          if (props.onCheck) {
                            props.onCheck([])
                          }
                        }}
                        className={classes.treeAction}
                        disabled={isCheckDisabled}
                      >
                        <FilterNoneIcon style={{padding: 1}}/>
                      </IconButton>
                    </span>
                  </Tooltip>
                </Fragment>
              )
              : null
            }
            {!withoutExpandControls
              ? (
                <Fragment>
                  <Tooltip title={t("components.enhancedTree.expandAll.tooltip")}>
                    <IconButton
                      className={classes.treeAction}
                      onClick={() => handleExpand(getKeys(filteredTree, childrenKey, getNodeKey))}
                    >
                      <HeightIcon/>
                    </IconButton>
                  </Tooltip>
                  <Tooltip title={t("components.enhancedTree.collapseAll.tooltip")}>
                    <IconButton
                      className={classes.treeAction}
                      onClick={() => handleExpand([])}
                    >
                      <VerticalAlignCenterIcon/>
                    </IconButton>
                  </Tooltip>
                </Fragment>
              )
              : null
            }
          </Grid>
          <Grid item xs={6} style={{padding: "8px 0"}}>
            {!withoutSearch
              ? (
                <AutoSearchInput
                  onSearch={handleSearch}
                />
              )
              : null
            }
          </Grid>
        </Grid>
      </div>
      {withLevelSelector && getKeys(filteredTree, childrenKey, getNodeKey).length > 1 && (
        <Fragment>
          <TreeView
            defaultCollapseIcon={<ExpandMoreIcon/>}
            defaultExpandIcon={<ChevronRightIcon/>}
            disableSelection
            onNodeToggle={(ev, nodeIds) => handleHeight(uuid, nodeIds, withLevelSelector)}
          >
            {levelTree.map(node => getTreeNode(node, ({id}) => id, true, checkable, (props.checkedKeys || checkedKeys), handleLevelCheck, getIsCheckable))}
          </TreeView>
          <Divider className={classes.divider}/>
        </Fragment>
      )}
      <div className={`enhanced-tree__tree ${classes.tree}`}>
        {(filteredTree && filteredTree.length > 0)
          ? (
            <TreeView
              defaultCollapseIcon={<ExpandMoreIcon/>}
              defaultExpandIcon={<ChevronRightIcon/>}
              disableSelection
              expanded={expandedKeys}
              onNodeToggle={(ev, nodeIds) => handleExpand(nodeIds)}
            >
              {filteredTree.map(node => getTreeNode(node, getNodeKey, false, checkable, (props.checkedKeys || checkedKeys), handleCheck, getIsCheckable))}
            </TreeView>
          )
          : <CustomEmpty/>
        }
      </div>
    </div>
  )
}

export default compose(
  withStyles(styles),
  connect(state => ({
    languages: state.app.languages,
    defaultLanguage: state.app.language
  }))
)(EnhancedTree)